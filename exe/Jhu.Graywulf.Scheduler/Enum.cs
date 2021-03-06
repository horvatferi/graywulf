﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    public enum RunningState
    {
        Up,
        Down
    }

    /// <summary>
    /// Job execution status
    /// </summary>
    public enum JobStatus
    {
        Unknown,
        Starting,
        Resuming,
        Executing,
        TimedOut,
        Persisted,
        Failed,
        Cancelled,
    }

    /// <summary>
    /// Possible workflow events
    /// </summary>
    public enum WorkflowEventType
    {
        /* TODO: delete
        /// <summary>
        /// Workflow has aborted without cancelation and persistence
        /// </summary>
        WorkflowAborted,
         * */

        /// <summary>
        /// Workflow completed without unhandled exceptions.
        /// </summary>
        Completed,

        /// <summary>
        /// Workflow forcefully cancelled, either by the user, either because of timeout.
        /// </summary>
        Cancelled,

        TimedOut,

        Failed,
        Persisted,
        
        //WorkflowIdle,
        
        //WorkflowUnloaded,
    }
}
