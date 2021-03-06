﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Hosting;
using Jhu.Graywulf.Logging;
using gw = Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Implements logic to direct requests to a specific server
    /// </summary>
    public class Scheduler : MarshalByRefObject, IScheduler
    {
        private object syncRoot;
        private QueueManager queueManager;

        internal Scheduler(QueueManager queueManager)
        {
            InitializeMembers();

            this.queueManager = queueManager;
        }

        private void InitializeMembers()
        {
            this.syncRoot = new object();
            this.queueManager = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This is required to prevent unloading the instance referenced by remoting clients only.
        /// </remarks>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void GetContextInfo(Guid workflowInstanceId, out Guid userGuid, out string userName, out Guid jobGuid, out string jobID)
        {
            Job job;
            lock (queueManager.RunningJobs)
            {
                job = queueManager.RunningJobs[workflowInstanceId];
            }

            userGuid = job.UserGuid;
            userName = job.UserName;
            jobGuid = job.Guid;
            jobID = job.JobID;
        }

        public Guid GetNextServerInstance(Guid[] databaseDefinitions, string databaseVersion, Guid[] databaseInstances)
        {
            var sis = GetServerInstancesInternal(databaseDefinitions, databaseVersion, databaseInstances);
            return sis[GetNextServerIndex(sis)].Guid;
        }

        public Guid[] GetServerInstances(Guid[] databaseDefinitions, string databaseVersion, Guid[] databaseInstances)
        {
            return GetServerInstancesInternal(databaseDefinitions, databaseVersion, databaseInstances).Select(x => x.Guid).ToArray();
        }

        public Guid GetNextDatabaseInstance(Guid databaseDefinition, string databaseVersion)
        {
            var q =
                from di in queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances[databaseVersion].Values
                where di.ServerInstance.IsAvailable
                select di;

            var dis = q.ToArray();

            var sis = new ServerInstance[dis.Length];
            for (int i = 0; i < sis.Length; i++)
            {
                sis[i] = dis[i].ServerInstance;
            }

            return sis[GetNextServerIndex(sis)].Guid;
        }

        public Guid[] GetDatabaseInstances(Guid databaseDefinition, string databaseVersion)
        {
            return queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances[databaseVersion].Keys.ToArray();
        }

        public Guid[] GetDatabaseInstances(Guid serverInstance, Guid databaseDefinition, string databaseVersion)
        {
            var q = from di in queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances[databaseVersion].Values
                    where di.ServerInstance.IsAvailable && di.ServerInstance.Guid == serverInstance
                    select di.Guid;

            return q.ToArray();
        }

        private int GetNextServerIndex(ServerInstance[] serverInstances)
        {
            lock (syncRoot)
            {
                // Find server with the earliest time stamp
                DateTime min = DateTime.MaxValue;
                int m = -1;

                for (int i = 0; i < serverInstances.Length; i++)
                {
                    ServerInstance si = serverInstances[i];

                    if (si.LastAssigned < min)
                    {
                        min = si.LastAssigned;
                        m = i;
                    }
                }

                if (m == -1)
                {
                    throw new SchedulerException(ExceptionMessages.NoServerForDatabaseFound);
                }

                serverInstances[m].LastAssigned = DateTime.Now;

                return m;
            }
        }

        /// <summary>
        /// Returns the server instances that all contain an instance
        /// the database definition
        /// </summary>
        /// <param name="databaseDefinitions"></param>
        /// <param name="databaseVersionName"></param>
        /// <returns></returns>
        private ServerInstance[] GetServerInstancesInternal(Guid[] databaseDefinitions, string databaseVersionName, Guid[] databaseInstances)
        {
            // TODO: lock?
            // TODO: for some reason it might return wrong results when database version is non-existing, check this

            // Start with all server instances
            var sis = new HashSet<Guid>(queueManager.Cluster.ServerInstances.Keys);

            // If there's any database instances specified then a specific server instance will be required
            if (databaseInstances != null && databaseInstances.Length > 0)
            {
                var disis = new HashSet<Guid>();
                foreach (var di in databaseInstances)
                {
                    var si = queueManager.Cluster.DatabaseInstances[di].ServerInstance;
                    if (si.IsAvailable)
                    {
                        disis.Add(si.Guid);
                    }
                }

                sis.IntersectWith(disis);
            }

            foreach (var dd in databaseDefinitions)
            {
                if (queueManager.Cluster.DatabaseDefinitions[dd].DatabaseInstances.ContainsKey(databaseVersionName))
                {
                    var disis = new HashSet<Guid>();

                    foreach (var di in queueManager.Cluster.DatabaseDefinitions[dd].DatabaseInstances[databaseVersionName].Values)
                    {
                        var si = di.ServerInstance;

                        if (si.IsAvailable)
                        {
                            disis.Add(si.Guid);
                        }
                    }

                    sis.IntersectWith(disis);
                }
            }


            var res = new ServerInstance[sis.Count];

            int q = 0;
            foreach (var si in sis)
            {
                res[q] = queueManager.Cluster.ServerInstances[si];
                q++;
            }

            return res;
        }
    }
}
