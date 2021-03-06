﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Types of entities as stored in the database
    /// </summary>
    public enum EntityType : int
    {
        // NOTE: Make sure that EntityType and PluralEntityNames use same values

        /// <summary>
        /// Default value for entity type
        /// </summary>
        Unknown = -1,

        #region Cluster

        /// <summary>
        /// SQL Server cluster
        /// </summary>
        Cluster = 0x00010000,

        /// <summary>
        /// Machine role that contains machines
        /// </summary>
        MachineRole = Cluster | 0x0100,

        /// <summary>
        /// Physical machine
        /// </summary>
        Machine = Cluster | 0x0200,

        /// <summary>
        /// Server version
        /// </summary>
        ServerVersion = Cluster | 0x0300,

        /// <summary>
        /// SQL Server instance
        /// </summary>
        ServerInstance = Cluster | 0x0400,

        /// <summary>
        /// Logical disk volume
        /// </summary>
        DiskVolume = Cluster | 0x0500,

        AllCluster = Cluster | 0xFFFF,

        #endregion
        #region Domain

        Domain = 0x00020000,
        //

        AllDomain = Domain | 0xFFFF,

        #endregion
        #region Federation

        /// <summary>
        /// Federation of databases (application)
        /// </summary>
        Federation = 0x00040000,

        /// <summary>
        /// Logical database definition
        /// </summary>
        DatabaseDefinition = Federation | 0x0100,

        /// <summary>
        /// Logical file group definition
        /// </summary>
        FileGroup = Federation | 0x0200,

        /// <summary>
        /// Slice limit definition
        /// </summary>
        Slice = Federation | 0x0300,

        /// <summary>
        /// Partition limit definition
        /// </summary>
        Partition = Federation | 0x0400,

        /// <summary>
        /// Database Version definition
        /// </summary>
        DatabaseVersion = Federation | 0x0500,

        /// <summary>
        /// Distributed partitioned view logical definition
        /// </summary>
        DistributedPartitionedView = Federation | 0x0600,

        /// <summary>
        /// Binary code deployment package
        /// </summary>
        DeploymentPackage = Federation | 0x0700,

        /// <summary>
        /// Remote database
        /// </summary>
        RemoteDatabase = Federation | 0x0800,

        AllFederation = Federation | 0xFFFF,

        #endregion
        #region Layout

        Layout = 0x00080000,

        /// <summary>
        /// Physical database instance
        /// </summary>
        DatabaseInstance = Layout | 0x0100,

        /// <summary>
        /// Physical database file group
        /// </summary>
        DatabaseInstanceFileGroup = Layout | 0x0200,

        /// <summary>
        /// Physical database file
        /// </summary>
        DatabaseInstanceFile = Layout | 0x0300,

        /// <summary>
        /// User database instance
        /// </summary>
        UserDatabaseInstance = Layout | 0x0400,

        AllLayou = Layout | 0xFFFF,

        #endregion
        #region Security

        Security = 0x00100000,

        /// <summary>
        /// System user group
        /// </summary>
        UserGroup = Security | 0x0100,

        /// <summary>
        /// User with a password
        /// </summary>
        User = Security | 0x0200,

        AllSecurity = Security | 0xFFFF,

        #endregion
        #region Jobs

        Jobs = 0x00200000,

        /// <summary>
        /// Scheduling queue definition
        /// </summary>
        QueueDefinition = Jobs | 0x0100,

        /// <summary>
        /// Scheduling queue instance
        /// </summary>
        QueueInstance = Jobs | 0x0200,

        /// <summary>
        /// Scheduler job definition
        /// </summary>
        JobDefinition = Jobs | 0x0300,

        /// <summary>
        /// Scheduler job instance
        /// </summary>
        JobInstance = Jobs | 0x0400,

        AllJobs = Jobs | 0xFFFF,

        #endregion
    }
}
