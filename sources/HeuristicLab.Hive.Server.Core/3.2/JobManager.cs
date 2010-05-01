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
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using HeuristicLab.DataAccess.Interfaces;
using System.Data;
using System.IO;
using HeuristicLab.Tracing;
using System.Transactions;
using HeuristicLab.Hive.Server.LINQDataAccess;
using IsolationLevel=System.Transactions.IsolationLevel;

namespace HeuristicLab.Hive.Server.Core {
  class JobManager: IJobManager, IInternalJobManager {

    //ISessionFactory factory;
    ILifecycleManager lifecycleManager;

    #region IJobManager Members

    public JobManager() {
      //factory = ServiceLocator.GetSessionFactory();
      lifecycleManager = ServiceLocator.GetLifecycleManager();

      lifecycleManager.RegisterStartup(new EventHandler(lifecycleManager_OnStartup));
      lifecycleManager.RegisterStartup(new EventHandler(lifecycleManager_OnShutdown));
    }

    private JobDto GetLastJobResult(Guid jobId) {     
      return DaoLocator.JobDao.FindById(jobId);
    }

    public void ResetJobsDependingOnResults(JobDto job) {
      Logger.Info("Setting job " + job.Id + " offline");
      if (job != null) {
        DaoLocator.JobDao.SetJobOffline(job);
      }
    }
         

    void checkForDeadJobs() {
      Logger.Info("Searching for dead Jobs");
      using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = ApplicationConstants.ISOLATION_LEVEL_SCOPE })) {
        List<JobDto> allJobs = new List<JobDto>(DaoLocator.JobDao.FindAll());
        foreach (JobDto curJob in allJobs) {
          if (curJob.State != State.calculating) {
            ResetJobsDependingOnResults(curJob);
          }
        }
        scope.Complete();
      }
      DaoLocator.DestroyContext();
    }

    void lifecycleManager_OnStartup(object sender, EventArgs e) {
      checkForDeadJobs();
    }

    void lifecycleManager_OnShutdown(object sender, EventArgs e) {
      checkForDeadJobs();
    }

    /// <summary>
    /// returns all jobs stored in the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<JobDto> GetAllJobs() {
         ResponseList<JobDto> response = new ResponseList<JobDto>();

         response.List = new List<JobDto>(DaoLocator.JobDao.FindAll());
         response.Success = true;
         response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ALL_JOBS;

         return response;
    }

    public ResponseList<JobDto> GetAllJobsWithFilter(State jobState, int offset, int count) {
      ResponseList<JobDto> response = new ResponseList<JobDto>();
      response.List = new List<JobDto>(DaoLocator.JobDao.FindWithLimitations(jobState, offset, count));
      return response;
    }  

    /// <summary>
    /// Gets the streamed job
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public Stream GetJobStreamById(Guid jobId) {           
      return DaoLocator.JobDao.GetSerializedJobStream(jobId);
      
    }

    /// <summary>
    /// returns the job with the specified id
    /// </summary>
    /// <returns></returns>
    public ResponseObject<JobDto> GetJobById(Guid jobId) {
        ResponseObject<JobDto> response = new ResponseObject<JobDto>();

      response.Obj = DaoLocator.JobDao.FindById(jobId);
        if (response.Obj != null) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_GET_JOB_BY_ID;
        } else {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
        }

        return response;
      }

    public ResponseObject<JobDto> GetJobByIdWithDetails(Guid jobId) {
      ResponseObject<JobDto> job = new ResponseObject<JobDto>();
      job.Obj = DaoLocator.JobDao.FindById(jobId);
      if (job.Obj != null) {
        job.Success = true;
        job.StatusMessage = ApplicationConstants.RESPONSE_JOB_GET_JOB_BY_ID;

        job.Obj.Client = DaoLocator.ClientDao.GetClientForJob(jobId);
      } else {
        job.Success = false;
        job.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
      }      
      return job;
    }

    public ResponseObject<JobDto> AddJobWithGroupStrings(SerializedJob job, IEnumerable<string> resources) {
      IClientGroupDao cgd = DaoLocator.ClientGroupDao;
      foreach (string res in resources) {
        foreach(ClientGroupDto cg in cgd.FindByName(res)) {
          job.JobInfo.AssignedResourceIds.Add(cg.Id);
        }
      }
      return AddNewJob(job);
    }

    /// <summary>
    /// Adds a new job into the database
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public ResponseObject<JobDto> AddNewJob(SerializedJob job) {
        ResponseObject<JobDto> response = new ResponseObject<JobDto>();

        if (job != null && job.JobInfo != null) {
          if (job.JobInfo.State != State.offline) {
            response.Success = false;
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOBSTATE_MUST_BE_OFFLINE;
            return response;
          }
          if (job.JobInfo.Id != Guid.Empty) {
            response.Success = false;
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ID_MUST_NOT_BE_SET;
            return response;
          }
          if (job.SerializedJobData == null) {
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_NULL;
            response.Success = false;
            return response;
          }

          job.JobInfo.DateCreated = DateTime.Now;
          DaoLocator.JobDao.InsertWithAttachedJob(job);
          DaoLocator.PluginInfoDao.InsertPluginDependenciesForJob(job.JobInfo);
          
          response.Success = true;
          response.Obj = job.JobInfo;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_ADDED;
        } else {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_NULL;
        }

        return response;
      }
    /*  finally {
        if (session != null)
          session.EndSession();
      }
    } */

    /// <summary>
    /// Removes a job from the database
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public Response RemoveJob(Guid jobId) {
        Response response = new Response();

      JobDto job = DaoLocator.JobDao.FindById(jobId);
        if (job == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
          return response;
        }
        DaoLocator.JobDao.Delete(job);
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_REMOVED;

        return response;
      }

    public ResponseObject<JobDto> GetLastJobResultOf(Guid jobId) {
      ResponseObject<JobDto> result =
        new ResponseObject<JobDto>();

       result.Obj =
         GetLastJobResult(jobId);
       result.Success =
         result.Obj != null;

       return result;
    }

    public ResponseObject<SerializedJob>
      GetLastSerializedJobResultOf(Guid jobId, bool requested) {

        ResponseObject<SerializedJob> response =
          new ResponseObject<SerializedJob>();

      JobDto job = DaoLocator.JobDao.FindById(jobId);
        if (requested && (job.State == State.requestSnapshot || job.State == State.requestSnapshotSent)) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_RESULT_NOT_YET_HERE;
          
          //tx.Commit();
          
          return response;
        }

        /*JobResult lastResult =
          jobResultsAdapter.GetLastResultOf(job.Id);*/

        //if (lastResult != null) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_RESULT_SENT;
          response.Obj = new SerializedJob();
          response.Obj.JobInfo = job;
          response.Obj.SerializedJobData =
            DaoLocator.JobDao.GetBinaryJobFile(jobId);
        //} else {
        //  response.Success = false;
        //}

        //tx.Commit();
        return response;
      }
      /*catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }    */


    public Response RequestSnapshot(Guid jobId) {
     // ISession session = factory.GetSessionForCurrentThread();
      Response response = new Response();
      
     /* try {
        IJobAdapter jobAdapter = session.GetDataAdapter<JobDto, IJobAdapter>();*/

        JobDto job = DaoLocator.JobDao.FindById(jobId);
        if (job.State == State.requestSnapshot || job.State == State.requestSnapshotSent) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_REQUEST_ALLREADY_SET;
          return response; // no commit needed
        }
        if (job.State != State.calculating) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_IS_NOT_BEEING_CALCULATED;
          return response; // no commit needed
        }
        // job is in correct state
        job.State = State.requestSnapshot;
        DaoLocator.JobDao.Update(job);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_REQUEST_SET;

        return response;
      }
    /*  finally {
        if (session != null)
          session.EndSession();
      }
    } */

    public Response AbortJob(Guid jobId) {
      //ISession session = factory.GetSessionForCurrentThread();
      Response response = new Response();

    /*  try {
        IJobAdapter jobAdapter = session.GetDataAdapter<JobDto, IJobAdapter>();*/

//        JobDto job = jobAdapter.GetById(jobId);
      JobDto job = DaoLocator.JobDao.FindById(jobId);
        if (job == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
          return response; // no commit needed
        }
        if (job.State == State.abort) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ABORT_REQUEST_ALLREADY_SET;
          return response; // no commit needed
        }
        if (job.State != State.calculating && job.State != State.requestSnapshot && job.State != State.requestSnapshotSent) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_IS_NOT_BEEING_CALCULATED;
          return response; // no commit needed
        }
        // job is in correct state
        job.State = State.abort;
      DaoLocator.JobDao.Update(job);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ABORT_REQUEST_SET;

        return response;
      }
      /*finally {
        if (session != null)
          session.EndSession();
      }
    }   */

    public ResponseList<JobResult> GetAllJobResults(Guid jobId) {
      return new ResponseList<JobResult>();
    }

    public ResponseList<ProjectDto> GetAllProjects() {
      return null;
    }

    private Response createUpdateProject(ProjectDto project) {
      return null;
    }

    public Response CreateProject(ProjectDto project) {
      return createUpdateProject(project);
    }

    public Response ChangeProject(ProjectDto project) {
      return createUpdateProject(project);
    }

    public Response DeleteProject(Guid projectId) {
      return null;
    }

    public ResponseList<JobDto> GetJobsByProject(Guid projectId) {
      return null;      
    }
    #endregion
  }
}
