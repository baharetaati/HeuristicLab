﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Clients.Access;

namespace HeuristicLab.Clients.Hive {
  [Item("Hive Administrator", "Hive Administrator")]
  public sealed class HiveAdminClient : IContent {
    private static HiveAdminClient instance;
    public static HiveAdminClient Instance {
      get {
        if (instance == null) instance = new HiveAdminClient();
        return instance;
      }
    }

    #region Properties
    private IItemList<Resource> resources;
    public IItemList<Resource> Resources {
      get { return resources; }
    }

    private IItemList<Downtime> downtimes;
    public IItemList<Downtime> Downtimes {
      get { return downtimes; }
    }

    private Guid downtimeForResourceId;
    public Guid DowntimeForResourceId {
      get { return downtimeForResourceId; }
      set {
        downtimeForResourceId = value;
        if (downtimes != null) {
          downtimes.Clear();
        }
      }
    }

    private IItemList<Project> projects;
    public IItemList<Project> Projects {
      get { return projects; }
    }

    private IItemList<AssignedProjectResource> projectResourceAssignments;
    public IItemList<AssignedProjectResource> ProjectResourceAssignments {
      get { return projectResourceAssignments; }
    }

    private Dictionary<Guid, HiveItemCollection<RefreshableJob>> jobs;
    public Dictionary<Guid, HiveItemCollection<RefreshableJob>> Jobs {
      get { return jobs; }
      set {
        if (value != jobs)
          jobs = value;
        }
    }

    private Dictionary<Guid, HashSet<Guid>> projectAncestors;
    public Dictionary<Guid, HashSet<Guid>> ProjectAncestors {
      get { return projectAncestors; }
    }

    private Dictionary<Guid, HashSet<Guid>> projectDescendants;
    public Dictionary<Guid, HashSet<Guid>> ProjectDescendants {
      get { return projectDescendants; }
    }

    private Dictionary<Guid, HashSet<Guid>> resourceAncestors;
    public Dictionary<Guid, HashSet<Guid>> ResourceAncestors {
      get { return resourceAncestors; }
    }

    private Dictionary<Guid, HashSet<Guid>> resourceDescendants;
    public Dictionary<Guid, HashSet<Guid>> ResourceDescendants {
      get { return resourceDescendants; }
    }

    private Dictionary<Guid, string> projectNames;
    public Dictionary<Guid, string> ProjectNames {
      get { return projectNames; }
    }

    private HashSet<Project> disabledParentProjects;
    public HashSet<Project> DisabledParentProjects {
      get { return disabledParentProjects; }
    }

    private Dictionary<Guid, string> resourceNames;
    public Dictionary<Guid, string> ResourceNames {
      get { return resourceNames; }
    }

    private HashSet<Resource> disabledParentResources;
    public HashSet<Resource> DisabledParentResources {
      get { return disabledParentResources; }
    }
    #endregion

    #region Events
    public event EventHandler Refreshing;
    private void OnRefreshing() {
      EventHandler handler = Refreshing;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Refreshed;
    private void OnRefreshed() {
      var handler = Refreshed;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    private HiveAdminClient() { }

    #region Refresh
    public void Refresh() {
      OnRefreshing();

      try {
        resources = new ItemList<Resource>();
        projects = new ItemList<Project>();
        projectResourceAssignments = new ItemList<AssignedProjectResource>();
        jobs = new Dictionary<Guid, HiveItemCollection<RefreshableJob>>();
        projectNames = new Dictionary<Guid, string>();
        resourceNames = new Dictionary<Guid, string>();

        projectAncestors = new Dictionary<Guid, HashSet<Guid>>();
        projectDescendants = new Dictionary<Guid, HashSet<Guid>>();
        resourceAncestors = new Dictionary<Guid, HashSet<Guid>>();
        resourceDescendants = new Dictionary<Guid, HashSet<Guid>>();

        HiveServiceLocator.Instance.CallHiveService(service => {
          service.GetSlaveGroupsForAdministration().ForEach(g => resources.Add(g));
          service.GetSlavesForAdministration().ForEach(s => resources.Add(s));
          service.GetProjectsForAdministration().ForEach(p => projects.Add(p));
          var projectIds = projects.Select(p => p.Id).ToList();
          if (projectIds.Any()) {
            service.GetAssignedResourcesForProjectsAdministration(projectIds)
              .ForEach(a => projectResourceAssignments.Add(a));
            projectIds.ForEach(p => jobs.Add(p, new HiveItemCollection<RefreshableJob>()));
            var unsortedJobs = service.GetJobsByProjectIds(projectIds)
              .OrderBy(x => x.DateCreated).ToList();

            unsortedJobs.Where(j => j.State == JobState.DeletionPending).ToList().ForEach(j => jobs[j.ProjectId].Add(new RefreshableJob(j)));
            unsortedJobs.Where(j => j.State == JobState.StatisticsPending).ToList().ForEach(j => jobs[j.ProjectId].Add(new RefreshableJob(j)));
            unsortedJobs.Where(j => j.State == JobState.Online).ToList().ForEach(j => jobs[j.ProjectId].Add(new RefreshableJob(j)));

            foreach (var job in jobs.SelectMany(x => x.Value))
              LoadLightweightJob(job);

            projectNames = service.GetProjectNames();
            resourceNames = service.GetResourceNames();
          }
        });

        UpdateResourceGenealogy();
        UpdateProjectGenealogy();
        RefreshDisabledParentProjects();
        RefreshDisabledParentResources();
      }
      catch {
        throw;
      }
      finally {
        OnRefreshed();
      }
    }

    //public void UpdateResourceGenealogy(IItemList<Resource> resources) {
    //  resourceAncestors.Clear();
    //  resourceDescendants.Clear();

    //  foreach (var r in resources) {
    //    resourceAncestors.Add(r.Id, new HashSet<Resource>());
    //    resourceDescendants.Add(r.Id, new HashSet<Resource>());
    //  }

    //  foreach (var r in resources) {
    //    var parentResourceId = r.ParentResourceId;
    //    while (parentResourceId != null) {
    //      var parent = resources.SingleOrDefault(x => x.Id == parentResourceId);
    //      if (parent != null) {
    //        resourceAncestors[r.Id].Add(parent);
    //        resourceDescendants[parent.Id].Add(r);
    //        parentResourceId = parent.ParentResourceId;
    //      } else {
    //        parentResourceId = null;
    //      }
    //    }
    //  }
    //}

    //public void UpdateProjectGenealogy(IItemList<Project> projects) {
    //  projectAncestors.Clear();
    //  projectDescendants.Clear();

    //  foreach (var p in projects) {
    //    projectAncestors.Add(p.Id, new HashSet<Project>());
    //    projectDescendants.Add(p.Id, new HashSet<Project>());
    //  }

    //  foreach (var p in projects) {
    //    var parentProjectId = p.ParentProjectId;
    //    while (parentProjectId != null) {
    //      var parent = projects.SingleOrDefault(x => x.Id == parentProjectId);
    //      if (parent != null) {
    //        projectAncestors[p.Id].Add(parent);
    //        projectDescendants[parent.Id].Add(p);
    //        parentProjectId = parent.ParentProjectId;
    //      } else {
    //        parentProjectId = null;
    //      }
    //    }
    //  }
    //}

    private void UpdateResourceGenealogy() {
      resourceAncestors.Clear();
      resourceDescendants.Clear();

      // fetch resource ancestor set
      HiveServiceLocator.Instance.CallHiveService(service => {
        var ra = service.GetResourceGenealogy();
        ra.Keys.ToList().ForEach(k => resourceAncestors.Add(k, new HashSet<Guid>()));
        resourceAncestors.Keys.ToList().ForEach(k => resourceAncestors[k].UnionWith(ra[k]));
      });

      // build resource descendant set
      resourceAncestors.Keys.ToList().ForEach(k => resourceDescendants.Add(k, new HashSet<Guid>()));
      foreach (var ra in resourceAncestors) {
        foreach (var ancestor in ra.Value) {
          resourceDescendants[ancestor].Add(ra.Key);
        }
      }
    }

    private void UpdateProjectGenealogy() {
      projectAncestors.Clear();
      projectDescendants.Clear();

      // fetch project ancestor list
      HiveServiceLocator.Instance.CallHiveService(service => {
        var pa = service.GetProjectGenealogy();
        pa.Keys.ToList().ForEach(k => projectAncestors.Add(k, new HashSet<Guid>()));
        projectAncestors.Keys.ToList().ForEach(k => projectAncestors[k].UnionWith(pa[k]));
      });

      // build project descendant list
      projectAncestors.Keys.ToList().ForEach(k => projectDescendants.Add(k, new HashSet<Guid>()));
      foreach (var pa in projectAncestors) {
        foreach (var ancestor in pa.Value) {
          projectDescendants[ancestor].Add(pa.Key);
        }
      }
    }

    private void RefreshDisabledParentProjects() {
      disabledParentProjects = new HashSet<Project>();

      foreach (var pid in projects
        .Where(x => x.ParentProjectId.HasValue)
        .SelectMany(x => projectAncestors[x.Id]).Distinct()
        .Where(x => !projects.Select(y => y.Id).Contains(x))) {
        var p = new Project();
        p.Id = pid;
        p.ParentProjectId = projectAncestors[pid].FirstOrDefault();
        p.Name = projectNames[pid];
        disabledParentProjects.Add(p);
      }
    }

    private void RefreshDisabledParentResources() {
      disabledParentResources = new HashSet<Resource>();

      foreach (var rid in resources
        .Where(x => x.ParentResourceId.HasValue)
        .SelectMany(x => resourceAncestors[x.Id]).Distinct()
        .Where(x => !resources.Select(y => y.Id).Contains(x))) {
        var r = new SlaveGroup();
        r.Id = rid;
        r.ParentResourceId = resourceAncestors[rid].FirstOrDefault();
        r.Name = resourceNames[rid];
        disabledParentResources.Add(r);
      }
    }

    public void RefreshJobs() {
      var projectIds = new List<Guid>();
      jobs = new Dictionary<Guid, HiveItemCollection<RefreshableJob>>();

      HiveServiceLocator.Instance.CallHiveService(service => {
        service.GetProjectsForAdministration().ForEach(p => projectIds.Add(p.Id));
        if(projectIds.Any()) {
          projectIds.ForEach(p => jobs.Add(p, new HiveItemCollection<RefreshableJob>()));
          var unsortedJobs = service.GetJobsByProjectIds(projectIds)
            .OrderBy(x => x.DateCreated).ToList();
          
          unsortedJobs.Where(j => j.State == JobState.DeletionPending).ToList().ForEach(j => jobs[j.ProjectId].Add(new RefreshableJob(j)));
          unsortedJobs.Where(j => j.State == JobState.StatisticsPending).ToList().ForEach(j => jobs[j.ProjectId].Add(new RefreshableJob(j)));
          unsortedJobs.Where(j => j.State == JobState.Online).ToList().ForEach(j => jobs[j.ProjectId].Add(new RefreshableJob(j)));

          foreach(var job in jobs.SelectMany(x => x.Value))
            LoadLightweightJob(job);
        }
      });
    }

    public static void LoadLightweightJob(RefreshableJob refreshableJob) {
      var job = refreshableJob.Job;
      var tasks = HiveServiceLocator.Instance.CallHiveService(s => s.GetLightweightJobTasksWithoutStateLog(job.Id));
      if (tasks != null && tasks.Count > 0 && tasks.All(x => x.Id != Guid.Empty)) {
        if (tasks.All(x =>
          x.State == TaskState.Finished 
          || x.State == TaskState.Aborted 
          || x.State == TaskState.Failed)) {
          refreshableJob.ExecutionState = ExecutionState.Stopped;
          refreshableJob.RefreshAutomatically = false;
        } else if (
          tasks
            .Where(x => x.ParentTaskId != null)
            .All(x =>
              x.State != TaskState.Waiting
              || x.State != TaskState.Transferring
              || x.State != TaskState.Calculating)
          && tasks
             .Where(x => x.ParentTaskId != null)
             .Any(x => x.State == TaskState.Paused)) {
          refreshableJob.ExecutionState = ExecutionState.Paused;
          refreshableJob.RefreshAutomatically = false;
        } else if (tasks.Any(x => x.State == TaskState.Calculating 
                                  || x.State == TaskState.Transferring 
                                  || x.State == TaskState.Waiting)) {
          refreshableJob.ExecutionState = ExecutionState.Started;
        }
      }
    }

    public void SortJobs() {
      for(int i = 0; i < jobs.Count; i++) {
        var projectId = jobs.Keys.ElementAt(i);
        var unsortedJobs = jobs.Values.ElementAt(i);

        var sortedJobs = new HiveItemCollection<RefreshableJob>();
        sortedJobs.AddRange(unsortedJobs.Where(j => j.Job.State == JobState.DeletionPending));
        sortedJobs.AddRange(unsortedJobs.Where(j => j.Job.State == JobState.StatisticsPending));
        sortedJobs.AddRange(unsortedJobs.Where(j => j.Job.State == JobState.Online));

        jobs[projectId] = sortedJobs;
      }
    }

    #endregion

    #region Refresh downtime calendar
    public void RefreshCalendar() {
      if (downtimeForResourceId != null && downtimeForResourceId != Guid.Empty) {
        OnRefreshing();

        try {
          downtimes = new ItemList<Downtime>();

          HiveServiceLocator.Instance.CallHiveService(service => {
            service.GetDowntimesForResource(downtimeForResourceId).ForEach(d => downtimes.Add(d));
          });
        }
        catch {
          throw;
        }
        finally {
          OnRefreshed();
        }
      }
    }
    #endregion

    #region Store
    public static void Store(IHiveItem item, CancellationToken cancellationToken) {
      if (item.Id == Guid.Empty) {
        if (item is SlaveGroup) {
          item.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddSlaveGroup((SlaveGroup)item));
        }
        if (item is Slave) {
          item.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddSlave((Slave)item));
        }
        if (item is Downtime) {
          item.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddDowntime((Downtime)item));
        }
        if (item is Project) {
          item.Id = HiveServiceLocator.Instance.CallHiveService(s => s.AddProject((Project)item));
        }
      } else {
        if (item is SlaveGroup) {
          HiveServiceLocator.Instance.CallHiveService((s) => s.UpdateSlaveGroup((SlaveGroup)item));
        }
        if (item is Slave) {
          HiveServiceLocator.Instance.CallHiveService((s) => s.UpdateSlave((Slave)item));
        }
        if (item is Downtime) {
          HiveServiceLocator.Instance.CallHiveService((s) => s.UpdateDowntime((Downtime)item));
        }
        if (item is Project) {
          HiveServiceLocator.Instance.CallHiveService((s) => s.UpdateProject((Project)item));
        }
      }
    }
    #endregion

    #region Delete
    public static void Delete(IHiveItem item) {
      if (item is SlaveGroup) {
        HiveServiceLocator.Instance.CallHiveService((s) => s.DeleteSlaveGroup(item.Id));
      } else if (item is Slave) {
        HiveServiceLocator.Instance.CallHiveService((s) => s.DeleteSlave(item.Id));
      } else if (item is Downtime) {
        HiveServiceLocator.Instance.CallHiveService((s) => s.DeleteDowntime(item.Id));
      } else if (item is Project) {
        HiveServiceLocator.Instance.CallHiveService((s) => s.DeleteProject(item.Id));
      }
    }

    public static void RemoveJobs(List<Guid> jobIds) {
      HiveServiceLocator.Instance.CallHiveService((s) => s.UpdateJobStates(jobIds, JobState.StatisticsPending));
    }
    #endregion

    #region Job Handling

    public static void ResumeJob(RefreshableJob refreshableJob) {
      HiveServiceLocator.Instance.CallHiveService(service => {
        foreach (HiveTask task in refreshableJob.GetAllHiveTasks()) {
          if (task.Task.State == TaskState.Paused) {
            service.RestartTask(task.Task.Id);
          }
        }
      });
      refreshableJob.ExecutionState = ExecutionState.Started;
    }

    public static void PauseJob(RefreshableJob refreshableJob) {
      HiveServiceLocator.Instance.CallHiveService(service => {
        foreach (HiveTask task in refreshableJob.GetAllHiveTasks()) {
          if (task.Task.State != TaskState.Finished && task.Task.State != TaskState.Aborted && task.Task.State != TaskState.Failed)
            service.PauseTask(task.Task.Id);
        }
      });
      refreshableJob.ExecutionState = ExecutionState.Paused;
    }

    public static void StopJob(RefreshableJob refreshableJob) {
      HiveServiceLocator.Instance.CallHiveService(service => {
        foreach (HiveTask task in refreshableJob.GetAllHiveTasks()) {
          if (task.Task.State != TaskState.Finished && task.Task.State != TaskState.Aborted && task.Task.State != TaskState.Failed)
            service.StopTask(task.Task.Id);
        }
      });
      refreshableJob.ExecutionState = ExecutionState.Stopped;
    }

    public static void RemoveJob(RefreshableJob refreshableJob) {
      HiveServiceLocator.Instance.CallHiveService((s) => {
        s.UpdateJobState(refreshableJob.Id, JobState.StatisticsPending);
      });
    }
    #endregion

    public void ResetDowntime() {
      downtimeForResourceId = Guid.Empty;
      if (downtimes != null) {
        downtimes.Clear();
      }
    }

    #region Helper
    public IEnumerable<Project> GetAvailableProjectAncestors(Guid id) {
      if (projectAncestors.ContainsKey(id)) return projects.Where(x => projectAncestors[id].Contains(x.Id));
      else return Enumerable.Empty<Project>();
    }

    public IEnumerable<Project> GetAvailableProjectDescendants(Guid id) {
      if(projectDescendants.ContainsKey(id)) return projects.Where(x => projectDescendants[id].Contains(x.Id));
      else return Enumerable.Empty<Project>();
    }

    public IEnumerable<Resource> GetAvailableResourceAncestors(Guid id) {
      if (resourceAncestors.ContainsKey(id)) return resources.Where(x => resourceAncestors[id].Contains(x.Id));
      else return Enumerable.Empty<Resource>();
    }

    public IEnumerable<Resource> GetAvailableResourceDescendants(Guid id) {
      if (resourceDescendants.ContainsKey(id)) return resources.Where(x => resourceDescendants[id].Contains(x.Id));
      else return Enumerable.Empty<Resource>();
    }

    public IEnumerable<Resource> GetDisabledResourceAncestors(IEnumerable<Resource> availableResources) {
      var missingParentIds = availableResources
        .Where(x => x.ParentResourceId.HasValue)
        .SelectMany(x => resourceAncestors[x.Id]).Distinct()
        .Where(x => !availableResources.Select(y => y.Id).Contains(x));

      return resources.OfType<SlaveGroup>().Union(disabledParentResources).Where(x => missingParentIds.Contains(x.Id));
    }

    public bool CheckAccessToAdminAreaGranted() {
      if(projects != null) {
        return projects.Count > 0;
      } else {
        bool accessGranted = false;
        HiveServiceLocator.Instance.CallHiveService(s => {
          accessGranted = s.CheckAccessToAdminAreaGranted();
        });
        return accessGranted;
      }
    }

    public bool CheckOwnershipOfResource(Resource res, Guid userId) {
      if (res == null || userId == Guid.Empty) return false;

      if (res.OwnerUserId == userId) {
        return true;
      } else if(resourceAncestors.ContainsKey(res.Id)) {
        return GetAvailableResourceAncestors(res.Id).Where(x => x.OwnerUserId == userId).Any();
      }

      return false;
    }

    public bool CheckOwnershipOfProject(Project pro, Guid userId) {
      if (pro == null || userId == Guid.Empty) return false;

      if (pro.OwnerUserId == userId) {
        return true;
      } else if (projectAncestors.ContainsKey(pro.Id)) {
        return GetAvailableProjectAncestors(pro.Id).Where(x => x.OwnerUserId == userId).Any();
      }

      return false;
    }

    public bool CheckOwnershipOfParentProject(Project pro, Guid userId) {
      if (pro == null || userId == Guid.Empty) return false;

      if(projectAncestors.ContainsKey(pro.Id)) {
        return GetAvailableProjectAncestors(pro.Id).Any(x => x.OwnerUserId == userId);
      }

      if (pro.ParentProjectId != null && pro.ParentProjectId != Guid.Empty) {
        var parent = projects.FirstOrDefault(x => x.Id == pro.ParentProjectId.Value);
        if (parent != null)
          return parent.OwnerUserId == userId || GetAvailableProjectAncestors(parent.Id).Any(x => x.OwnerUserId == userId);
      }

      return false;
    }

    public bool CheckParentChange(Project child, Project parent) {
      bool changePossible = true;

      // change is not possible...
      // ... if the moved project is null
      // ... or the new parent is not stored yet
      // ... or there is not parental change
      if (child == null
          || (parent != null && parent.Id == Guid.Empty)
          || (parent != null && parent.Id == child.ParentProjectId)) {
        changePossible = false;
      } else if (parent == null && !IsAdmin()) {
        // ... if parent is null, but user is no admin (only admins are allowed to create root projects)
        changePossible = false;
      } else if (parent != null && (!IsAdmin() && parent.OwnerUserId != UserInformation.Instance.User.Id && !CheckOwnershipOfParentProject(parent, UserInformation.Instance.User.Id))) {
        // ... if the user is no admin nor owner of the new parent or grand..grandparents
        changePossible = false;
      } else if(parent != null && projectDescendants.ContainsKey(child.Id)) {
        // ... if the new parent is among the moved project's descendants
        changePossible = !GetAvailableProjectDescendants(child.Id).Where(x => x.Id == parent.Id).Any();
      }

      return changePossible;
    }

    public bool CheckParentChange(Resource child, Resource parent) {
      bool changePossible = true;

      // change is not possisble...
      // ... if the child resource is null
      // ... or the child resource equals the parent
      // ... or the new parent is not stored yet
      // ... or the new parent is a slave
      // ... or there is not parental change
      if (child == null 
        || child == parent
        || (parent != null && parent.Id == Guid.Empty)
        || (parent != null && parent is Slave)
        || (parent != null && parent.Id == child.ParentResourceId)) {
        changePossible = false;
      } else if (parent != null && resourceDescendants.ContainsKey(child.Id)) {
        // ... or if the new parent is among the moved resource's descendants
        changePossible = !GetAvailableResourceDescendants(child.Id).Where(x => x.Id == parent.Id).Any();
      }

      return changePossible;
    }

    private bool IsAdmin() {
      return HiveRoles.CheckAdminUserPermissions();
    }
    #endregion
  }
}
