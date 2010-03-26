﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Core;
using HeuristicLab.Hive.Server.DataAccess;
using System.Resources;
using System.Reflection;
using HeuristicLab.Hive.JobBase;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using System.Threading;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.DataAccess.Interfaces;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HeuristicLab.Tracing;
using Linq = HeuristicLab.Hive.Server.LINQDataAccess;
using System.Transactions;
using HeuristicLab.Hive.Server.LINQDataAccess;

namespace HeuristicLab.Hive.Server.Core {
  /// <summary>
  /// The ClientCommunicator manages the whole communication with the client
  /// </summary>
  public class ClientCommunicator : IClientCommunicator,
    IInternalClientCommunicator {
    private static Dictionary<Guid, DateTime> lastHeartbeats =
      new Dictionary<Guid, DateTime>();
    private static Dictionary<Guid, int> newAssignedJobs =
      new Dictionary<Guid, int>();
    private static Dictionary<Guid, int> pendingJobs =
      new Dictionary<Guid, int>();

    private static ReaderWriterLockSlim heartbeatLock =
      new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    //private ISessionFactory factory;
    private ILifecycleManager lifecycleManager;
    private IInternalJobManager jobManager;
    private IScheduler scheduler;

    private static int PENDING_TIMEOUT = 100;

    /// <summary>
    /// Initialization of the Adapters to the database
    /// Initialization of Eventhandler for the lifecycle management
    /// Initialization of lastHearbeats Dictionary
    /// </summary>
    public ClientCommunicator() {
      //factory = ServiceLocator.GetSessionFactory();

      lifecycleManager = ServiceLocator.GetLifecycleManager();
      jobManager = ServiceLocator.GetJobManager() as
        IInternalJobManager;
      scheduler = ServiceLocator.GetScheduler();

      lifecycleManager.RegisterHeartbeat(
        new EventHandler(lifecycleManager_OnServerHeartbeat));
    }

    /// <summary>
    /// Check if online clients send their hearbeats
    /// if not -> set them offline and check if they where calculating a job
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void lifecycleManager_OnServerHeartbeat(object sender, EventArgs e) {
      HiveLogger.Info(this.ToString() + ": Server Heartbeat ticked");

      using (TransactionScope scope = new TransactionScope()) {

        List<ClientDto> allClients = new List<ClientDto>(DaoLocator.ClientDao.FindAll());

        foreach (ClientDto client in allClients) {
          if (client.State != State.offline && client.State != State.nullState) {
            heartbeatLock.EnterUpgradeableReadLock();

            if (!lastHeartbeats.ContainsKey(client.Id)) {
              HiveLogger.Info(this.ToString() + ": Client " + client.Id +
                              " wasn't offline but hasn't sent heartbeats - setting offline");
              client.State = State.offline;
              DaoLocator.ClientDao.Update(client);
              HiveLogger.Info(this.ToString() + ": Client " + client.Id +
                              " wasn't offline but hasn't sent heartbeats - Resetting all his jobs");
              foreach (JobDto job in DaoLocator.JobDao.FindActiveJobsOfClient(client)) {
                //maybe implementa n additional Watchdog? Till then, just set them offline..
                DaoLocator.JobDao.SetJobOffline(job);                
                //jobManager.ResetJobsDependingOnResults(job);
              }
            } else {
              DateTime lastHbOfClient = lastHeartbeats[client.Id];

              TimeSpan dif = DateTime.Now.Subtract(lastHbOfClient);
              // check if time between last hearbeat and now is greather than HEARTBEAT_MAX_DIF
              if (dif.TotalSeconds > ApplicationConstants.HEARTBEAT_MAX_DIF) {
                // if client calculated jobs, the job must be reset
                HiveLogger.Info(this.ToString() + ": Client timed out and is on RESET");
                foreach (JobDto job in DaoLocator.JobDao.FindActiveJobsOfClient(client)) {
                  jobManager.ResetJobsDependingOnResults(job);
                  lock (newAssignedJobs) {
                    if (newAssignedJobs.ContainsKey(job.Id))
                      newAssignedJobs.Remove(job.Id);
                  }
                }

                // client must be set offline
                client.State = State.offline;

                //clientAdapter.Update(client);
                DaoLocator.ClientDao.Update(client);

                heartbeatLock.EnterWriteLock();
                lastHeartbeats.Remove(client.Id);
                heartbeatLock.ExitWriteLock();
              }
            }

            heartbeatLock.ExitUpgradeableReadLock();
          } else {
            //TODO: RLY neccesary?
            //HiveLogger.Info(this.ToString() + ": Client " + client.Id + " has wrong state: Shouldn't have offline or nullstate, has " + client.State);
            heartbeatLock.EnterWriteLock();
            //HiveLogger.Info(this.ToString() + ": Client " + client.Id + " has wrong state: Resetting all his jobs");
            if (lastHeartbeats.ContainsKey(client.Id))
              lastHeartbeats.Remove(client.Id);
            foreach (JobDto job in DaoLocator.JobDao.FindActiveJobsOfClient(client)) {
              jobManager.ResetJobsDependingOnResults(job);
            }
            heartbeatLock.ExitWriteLock();
          }
        }
        CheckForPendingJobs();
        DaoLocator.DestroyContext();
        scope.Complete();
      }
    }

    private void CheckForPendingJobs() {
      IList<JobDto> pendingJobsInDB = new List<JobDto>(DaoLocator.JobDao.GetJobsByState(State.pending));

      foreach (JobDto currJob in pendingJobsInDB) {
        lock (pendingJobs) {
          if (pendingJobs.ContainsKey(currJob.Id)) {
            if (pendingJobs[currJob.Id] <= 0) {
              currJob.State = State.offline;
              DaoLocator.JobDao.Update(currJob);              
            } else {
              pendingJobs[currJob.Id]--;
            }
          }
        }
      }
    }

    #region IClientCommunicator Members

    /// <summary>
    /// Login process for the client
    /// A hearbeat entry is created as well (login is the first hearbeat)
    /// </summary>
    /// <param name="clientInfo"></param>
    /// <returns></returns>
    public Response Login(ClientDto clientInfo) {
      Response response = new Response();

      heartbeatLock.EnterWriteLock();
      if (lastHeartbeats.ContainsKey(clientInfo.Id)) {
        lastHeartbeats[clientInfo.Id] = DateTime.Now;
      } else {
        lastHeartbeats.Add(clientInfo.Id, DateTime.Now);
      }
      heartbeatLock.ExitWriteLock();

      ClientDto dbClient = DaoLocator.ClientDao.FindById(clientInfo.Id);

      //Really set offline? 
      //Reconnect issues with the currently calculating jobs
      clientInfo.State = State.idle;
      clientInfo.CalendarSyncStatus = dbClient != null ? dbClient.CalendarSyncStatus : CalendarState.NotAllowedToFetch;

      if (dbClient == null)
        DaoLocator.ClientDao.Insert(clientInfo);
      else
        DaoLocator.ClientDao.Update(clientInfo);
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGIN_SUCCESS;
      return response;
    }

    public ResponseCalendar GetCalendar(Guid clientId) {
      ResponseCalendar response = new ResponseCalendar();

      ClientDto client = DaoLocator.ClientDao.FindById(clientId);
      if(client == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_RESOURCE_NOT_FOUND;
        return response;
      }
      
      response.ForceFetch = (client.CalendarSyncStatus == CalendarState.ForceFetch);
      
      IEnumerable<AppointmentDto> appointments = DaoLocator.UptimeCalendarDao.GetCalendarForClient(client);
      if (appointments.Count() == 0) {
        response.StatusMessage = ApplicationConstants.RESPONSE_UPTIMECALENDAR_NO_CALENDAR_FOUND;
        response.Success = false;
      } else {
        response.Success = true;
        response.Appointments = appointments;
      }

      client.CalendarSyncStatus = CalendarState.Fetched;
      DaoLocator.ClientDao.Update(client);
      return response;
    }

    public Response SetCalendarStatus(Guid clientId, CalendarState state) {
      Response response = new Response();      
      ClientDto client = DaoLocator.ClientDao.FindById(clientId);
      if (client == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_RESOURCE_NOT_FOUND;
        return response;
      }
      
      client.CalendarSyncStatus = state;
      DaoLocator.ClientDao.Update(client);
      
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_UPTIMECALENDAR_STATUS_UPDATED;
      
      return response;
    }

    /// <summary>
    /// The client has to send regulary heartbeats
    /// this hearbeats will be stored in the heartbeats dictionary
    /// check if there is work for the client and send the client a response if he should pull a job
    /// </summary>
    /// <param name="hbData"></param>
    /// <returns></returns>
    public ResponseHB ProcessHeartBeat(HeartBeatData hbData) {
      HiveLogger.Debug(this.ToString() + ": BEGIN Processing Heartbeat for Client " + hbData.ClientId);
      HiveLogger.Debug(this.ToString() + ": BEGIN Fetching Adapters");
      HiveLogger.Debug(this.ToString() + ": END Fetched Adapters");
      HiveLogger.Debug(this.ToString() + ": BEGIN Starting Transaction");

      HiveLogger.Debug(this.ToString() + ": END Started Transaction");

      ResponseHB response = new ResponseHB();
      response.ActionRequest = new List<MessageContainer>();

      HiveLogger.Debug(this.ToString() + ": BEGIN Started Client Fetching");
      ClientDto client = DaoLocator.ClientDao.FindById(hbData.ClientId);
      HiveLogger.Debug(this.ToString() + ": END Finished Client Fetching");
      // check if the client is logged in
      if (client.State == State.offline || client.State == State.nullState) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_USER_NOT_LOGGED_IN;
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.NoMessage));

        HiveLogger.Error(this.ToString() + " ProcessHeartBeat: Client state null or offline: " + client);

        return response;
      }

      client.NrOfFreeCores = hbData.FreeCores;
      client.FreeMemory = hbData.FreeMemory;

      // save timestamp of this heartbeat
      HiveLogger.Debug(this.ToString() + ": BEGIN Locking for Heartbeats");
      heartbeatLock.EnterWriteLock();
      HiveLogger.Debug(this.ToString() + ": END Locked for Heartbeats");
      if (lastHeartbeats.ContainsKey(hbData.ClientId)) {
        lastHeartbeats[hbData.ClientId] = DateTime.Now;
      } else {
        lastHeartbeats.Add(hbData.ClientId, DateTime.Now);
      }
      heartbeatLock.ExitWriteLock();

      HiveLogger.Debug(this.ToString() + ": BEGIN Processing Heartbeat Jobs");
      processJobProcess(hbData, response);
      HiveLogger.Debug(this.ToString() + ": END Processed Heartbeat Jobs");

      //check if new Cal must be loaded
      if (client.CalendarSyncStatus == CalendarState.Fetch || client.CalendarSyncStatus == CalendarState.ForceFetch) {
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_FETCH_OR_FORCEFETCH_CALENDAR;
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.FetchOrForceFetchCalendar));
        
        //client.CalendarSyncStatus = CalendarState.Fetching;
        
        HiveLogger.Info(this.ToString() + " fetch or forcefetch sent");        
      }

      // check if client has a free core for a new job
      // if true, ask scheduler for a new job for this client
      HiveLogger.Debug(this.ToString() + ": BEGIN Looking for Client Jobs");
      if (hbData.FreeCores > 0 && scheduler.ExistsJobForClient(hbData)) {
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.FetchJob));
      } else {
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.NoMessage));
      }
      HiveLogger.Debug(this.ToString() + ": END Looked for Client Jobs");
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_HEARTBEAT_RECEIVED;

      DaoLocator.ClientDao.Update(client);

      //tx.Commit();
      HiveLogger.Debug(this.ToString() + ": END Processed Heartbeat for Client " + hbData.ClientId);
      return response;
    }

    /// <summary>
    /// Process the Job progress sent by a client
    /// </summary>
    /// <param name="hbData"></param>
    /// <param name="jobAdapter"></param>
    /// <param name="clientAdapter"></param>
    /// <param name="response"></param>
    private void processJobProcess(HeartBeatData hbData, ResponseHB response) {
      HiveLogger.Info(this.ToString() + " processJobProcess: Started for Client " + hbData.ClientId);
      List<JobDto> jobsOfClient = new List<JobDto>(DaoLocator.JobDao.FindActiveJobsOfClient(DaoLocator.ClientDao.FindById(hbData.ClientId)));
      if (hbData.JobProgress != null && hbData.JobProgress.Count > 0) {        
        if (jobsOfClient == null || jobsOfClient.Count == 0) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_IS_NOT_BEEING_CALCULATED;
          HiveLogger.Error(this.ToString() + " processJobProcess: There is no job calculated by this user " + hbData.ClientId);
          return;
        }

        foreach (KeyValuePair<Guid, double> jobProgress in hbData.JobProgress) {
          JobDto curJob = DaoLocator.JobDao.FindById(jobProgress.Key);
          curJob.Client = DaoLocator.ClientDao.GetClientForJob(curJob.Id);
          if (curJob.Client == null || curJob.Client.Id != hbData.ClientId) {
            response.Success = false;
            response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_IS_NOT_BEEING_CALCULATED;
            HiveLogger.Error(this.ToString() + " processJobProcess: There is no job calculated by this user " + hbData.ClientId + " Job: " + curJob);
          } else if (curJob.State == State.abort) {
            // a request to abort the job has been set
            response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.AbortJob, curJob.Id));
            curJob.State = State.finished;
          } else {
            // save job progress
            curJob.Percentage = jobProgress.Value;

            if (curJob.State == State.requestSnapshot) {
              // a request for a snapshot has been set
              response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.RequestSnapshot, curJob.Id));
              curJob.State = State.requestSnapshotSent;
            }
          }
          DaoLocator.JobDao.Update(curJob);
        }
       }
      foreach (JobDto currJob in jobsOfClient) {
        bool found = false;
        if(hbData.JobProgress != null) {
          foreach (Guid jobId in hbData.JobProgress.Keys) {
            if (jobId == currJob.Id) {
              found = true;
              break;
            }
          }
        }
        if (!found) {
          lock (newAssignedJobs) {
            if (newAssignedJobs.ContainsKey(currJob.Id)) {
              newAssignedJobs[currJob.Id]--;
              HiveLogger.Error(this.ToString() + " processJobProcess: Job TTL Reduced by one for job: " + currJob + "and is now: " + newAssignedJobs[currJob.Id] + ". User that sucks: " + currJob.Client);
              if (newAssignedJobs[currJob.Id] <= 0) {
                HiveLogger.Error(this.ToString() + " processJobProcess: Job TTL reached Zero, Job gets removed: " + currJob + " and set back to offline. User that sucks: " + currJob.Client);

                currJob.State = State.offline;
                DaoLocator.JobDao.Update(currJob);

                response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.AbortJob, currJob.Id));

                newAssignedJobs.Remove(currJob.Id);
              }
            } else {
              HiveLogger.Error(this.ToString() + " processJobProcess: Job ID wasn't with the heartbeats:  " + currJob);
              currJob.State = State.offline;
              DaoLocator.JobDao.Update(currJob);
            }
          } // lock
        } else {
          lock (newAssignedJobs) {

            if (newAssignedJobs.ContainsKey(currJob.Id)) {
              HiveLogger.Info(this.ToString() + " processJobProcess: Job is sending a heart beat, removing it from the newAssignedJobList: " + currJob);
              newAssignedJobs.Remove(currJob.Id);
            }
          }
        }
      }
    }

    /// <summary>
    /// if the client was told to pull a job he calls this method
    /// the server selects a job and sends it to the client
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    /*public ResponseSerializedJob SendSerializedJob(Guid clientId) {
      ISession session = factory.GetSessionForCurrentThread();
      ITransaction tx = null;

      try {
        IJobAdapter jobAdapter =
          session.GetDataAdapter<JobDto, IJobAdapter>();

        tx = session.BeginTransaction();

        ResponseSerializedJob response = new ResponseSerializedJob();

        JobDto job2Calculate = scheduler.GetNextJobForClient(clientId);
        if (job2Calculate != null) {
          SerializedJob computableJob =
            jobAdapter.GetSerializedJob(job2Calculate.Id);

          response.Job = computableJob;
          response.Success = true;
          HiveLogger.Info(this.ToString() + " SendSerializedJob: Job pulled: " + computableJob.JobInfo + " for user " + clientId);                      
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_PULLED;
          lock (newAssignedJobs) {
            if (!newAssignedJobs.ContainsKey(job2Calculate.Id))
              newAssignedJobs.Add(job2Calculate.Id, ApplicationConstants.JOB_TIME_TO_LIVE);
          }
        } else {
          HiveLogger.Info(this.ToString() + " SendSerializedJob: No more Jobs left for " + clientId);                      
          response.Success = false;
          response.Job = null;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOBS_LEFT;
        }

        tx.Commit();

        return response;
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }     */

    /// <summary>
    /// if the client was told to pull a job he calls this method
    /// the server selects a job and sends it to the client
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public ResponseJob SendJob(Guid clientId) {

      ResponseJob response = new ResponseJob();

      JobDto job2Calculate = scheduler.GetNextJobForClient(clientId);
      if (job2Calculate != null) {
        response.Job = job2Calculate;
        response.Success = true;
        HiveLogger.Info(this.ToString() + " SendSerializedJob: Job pulled: " + job2Calculate + " for user " + clientId);
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_PULLED;
        lock (newAssignedJobs) {
          if (!newAssignedJobs.ContainsKey(job2Calculate.Id))
            newAssignedJobs.Add(job2Calculate.Id, ApplicationConstants.JOB_TIME_TO_LIVE);
        }
      } else {
        response.Success = false;
        response.Job = null;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOBS_LEFT;
        HiveLogger.Info(this.ToString() + " SendSerializedJob: No more Jobs left for " + clientId);
      }



      return response;
    }

    public ResponseResultReceived ProcessJobResult(
      Stream stream,
      bool finished) {

      HiveLogger.Info(this.ToString() + " ProcessJobResult: BEGIN Job received for Storage - main method:");


      Stream jobResultStream = null;
      Stream jobStream = null;

      //try {
      BinaryFormatter formatter =
        new BinaryFormatter();

      JobResult result =
        (JobResult)formatter.Deserialize(stream);

      //important - repeatable read isolation level is required here, 
      //otherwise race conditions could occur when writing the stream into the DB
      //just removed TransactionIsolationLevel.RepeatableRead
      //tx = session.BeginTransaction();

      ResponseResultReceived response =
        ProcessJobResult(
        result.ClientId,
        result.JobId,
        new byte[] { },
        result.Percentage,
        result.Exception,
        finished);

      if (response.Success) {
        jobStream = DaoLocator.JobDao.GetSerializedJobStream(result.JobId);

        byte[] buffer = new byte[3024];
        int read = 0;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
          jobStream.Write(buffer, 0, read);
        }
        jobStream.Close();
      }
      HiveLogger.Info(this.ToString() + " ProcessJobResult: END Job received for Storage:");
      return response;
    }


    private ResponseResultReceived ProcessJobResult(Guid clientId,
      Guid jobId,
      byte[] result,
      double percentage,
      Exception exception,
      bool finished) {

      HiveLogger.Info(this.ToString() + " ProcessJobResult: BEGIN Job received for Storage - SUB method: " + jobId);

      ResponseResultReceived response = new ResponseResultReceived();
      ClientDto client =
        DaoLocator.ClientDao.FindById(clientId);

      SerializedJob job =
        new SerializedJob();

      if (job != null) {
        job.JobInfo =
          DaoLocator.JobDao.FindById(jobId);
        job.JobInfo.Client = job.JobInfo.Client = DaoLocator.ClientDao.GetClientForJob(jobId);
      }

      if (job == null && job.JobInfo != null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOB_WITH_THIS_ID;
        response.JobId = jobId;

        HiveLogger.Error(this.ToString() + " ProcessJobResult: No job with Id " + jobId);

        //tx.Rollback();
        return response;
      }
      if (job.JobInfo.State == State.abort) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_WAS_ABORTED;

        HiveLogger.Error(this.ToString() + " ProcessJobResult: Job was aborted! " + job.JobInfo);

        //tx.Rollback();
        return response;
      }
      if (job.JobInfo.Client == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_IS_NOT_BEEING_CALCULATED;
        response.JobId = jobId;

        HiveLogger.Error(this.ToString() + " ProcessJobResult: Job is not being calculated (client = null)! " + job.JobInfo);

        //tx.Rollback();
        return response;
      }
      if (job.JobInfo.Client.Id != clientId) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_WRONG_CLIENT_FOR_JOB;
        response.JobId = jobId;

        HiveLogger.Error(this.ToString() + " ProcessJobResult: Wrong Client for this Job! " + job.JobInfo + ", Sending Client is: " + clientId);

        //tx.Rollback();
        return response;
      }
      if (job.JobInfo.State == State.finished) {
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED;
        response.JobId = jobId;

        HiveLogger.Error(this.ToString() + " ProcessJobResult: Job already finished! " + job.JobInfo + ", Sending Client is: " + clientId);

        //tx.Rollback();
        return response;
      }
      if (job.JobInfo.State == State.requestSnapshotSent) {
        job.JobInfo.State = State.calculating;
      }
      if (job.JobInfo.State != State.calculating &&
        job.JobInfo.State != State.pending) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_WRONG_JOB_STATE;
        response.JobId = jobId;

        HiveLogger.Error(this.ToString() + " ProcessJobResult: Wrong Job State, job is: " + job.JobInfo);

        //tx.Rollback();
        return response;
      }
      job.JobInfo.Percentage = percentage;

      if (finished) {
        job.JobInfo.State = State.finished;
      }

      job.SerializedJobData = result;

      DaoLocator.JobDao.Update(job.JobInfo);

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED;
      response.JobId = jobId;
      response.finished = finished;

      HiveLogger.Info(this.ToString() + " ProcessJobResult: END Job received for Storage - SUB method: " + jobId);
      return response;

    }


    /// <summary>
    /// the client can send job results during calculating 
    /// and will send a final job result when he finished calculating
    /// these job results will be stored in the database
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="jobId"></param>
    /// <param name="result"></param>
    /// <param name="exception"></param>
    /// <param name="finished"></param>
    /// <returns></returns>
    public ResponseResultReceived StoreFinishedJobResult(Guid clientId,
      Guid jobId,
      byte[] result,
      double percentage,
      Exception exception) {

      return ProcessJobResult(clientId, jobId, result, percentage, exception, true);
    }


    public ResponseResultReceived ProcessSnapshot(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception) {
      return ProcessJobResult(clientId, jobId, result, percentage, exception, false);
    }

    /// <summary>
    /// when a client logs out the state will be set
    /// and the entry in the last hearbeats dictionary will be removed
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>                       
    public Response Logout(Guid clientId) {

      HiveLogger.Info("Client logged out " + clientId);
      
      Response response = new Response();

      heartbeatLock.EnterWriteLock();
      if (lastHeartbeats.ContainsKey(clientId))
        lastHeartbeats.Remove(clientId);
      heartbeatLock.ExitWriteLock();

      ClientDto client = DaoLocator.ClientDao.FindById(clientId);
      if (client == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGOUT_CLIENT_NOT_REGISTERED;
        return response;
      }
      if (client.State == State.calculating) {
        // check wich job the client was calculating and reset it
        IEnumerable<JobDto> jobsOfClient = DaoLocator.JobDao.FindActiveJobsOfClient(client);
        foreach (JobDto job in jobsOfClient) {
          if (job.State != State.finished)
            jobManager.ResetJobsDependingOnResults(job);
        }
      }

      client.State = State.offline;
      DaoLocator.ClientDao.Update(client);

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGOUT_SUCCESS;

      return response;
    } 

    /// <summary>
    /// If a client goes offline and restores a job he was calculating 
    /// he can ask the client if he still needs the job result
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public Response IsJobStillNeeded(Guid jobId) {
      Response response = new Response();
      JobDto job = DaoLocator.JobDao.FindById(jobId);
      if (job == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_DOESNT_EXIST;
        HiveLogger.Error(this.ToString() + " IsJobStillNeeded: Job doesn't exist (anymore)! " + jobId);
        return response;
      }
      if (job.State == State.finished) {
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_ALLREADY_FINISHED;
        HiveLogger.Error(this.ToString() + " IsJobStillNeeded: already finished! " + job);
        return response;
      }
      job.State = State.pending;
      lock (pendingJobs) {
        pendingJobs.Add(job.Id, PENDING_TIMEOUT);
      }

      DaoLocator.JobDao.Update(job);

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_SEND_JOBRESULT;
      return response;
    }

    public ResponsePlugin SendPlugins(List<HivePluginInfoDto> pluginList) {
      ResponsePlugin response = new ResponsePlugin();
      foreach (HivePluginInfoDto pluginInfo in pluginList) {
        // TODO: BuildDate deleted, not needed???
        // TODO: Split version to major, minor and revision number
        foreach (IPluginDescription currPlugin in ApplicationManager.Manager.Plugins) {
          if (currPlugin.Name == pluginInfo.Name) {

            CachedHivePluginInfoDto currCachedPlugin = new CachedHivePluginInfoDto {
              Name = currPlugin.Name,
              Version = currPlugin.Version.ToString(),
              BuildDate = currPlugin.BuildDate
            };

            foreach (string fileName in from file in currPlugin.Files select file.Name) {
              currCachedPlugin.PluginFiles.Add(File.ReadAllBytes(fileName));
            }
            response.Plugins.Add(currCachedPlugin);
          }
        }
      }
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_PLUGINS_SENT;

      return response;

    }

    #endregion
  }
}
