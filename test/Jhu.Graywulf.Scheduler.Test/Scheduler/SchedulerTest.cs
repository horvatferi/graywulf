﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler.Test
{
    [TestClass]
    public class SchedulerTest : TestClassBase
    {
        [TestMethod]
        public void StartStopTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                PurgeTestJobs();

                SchedulerTester.Instance.EnsureRunning();
                Thread.Sleep(new TimeSpan(0, 0, 20));
                SchedulerTester.Instance.DrainStop();
            }
        }

        [TestMethod]
        public void SimpleJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(new TimeSpan(0, 0, 10), JobType.AtomicDelay, QueueType.Long);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
                Assert.AreEqual("OK", (string)job.Parameters["Result"].GetValue());
            }
        }


        [TestMethod]
        public void CancelAtomicJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.AtomicDelay, QueueType.Long);

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                CancelJob(guid);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
            }
        }

        [TestMethod]
        public void CancelCancelableJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                // Make delay long enough but cancelable
                var guid = ScheduleTestJob(new TimeSpan(0, 10, 0), JobType.CancelableDelay, QueueType.Long);

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                CancelJob(guid);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
            }
        }

        [TestMethod]
        public void KillSchedulerTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                // Make delay long enough but cancelable
                var guid = ScheduleTestJob(new TimeSpan(0, 10, 0), JobType.CancelableDelay, QueueType.Long);

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                SchedulerTester.Instance.Kill();

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                // This will complete because cancellation is not possible
                Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
            }
        }

        [TestMethod]
        public void TimeOutCancelableTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                // Time must be longer than the time-out of quick queue!
                var guid = ScheduleTestJob(new TimeSpan(0, 1, 0), JobType.CancelableDelay, QueueType.Quick);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                Assert.IsTrue(ji.JobExecutionStatus == JobExecutionState.TimedOut ||
                    ji.JobExecutionStatus == JobExecutionState.Completed);
            }
        }

        [TestMethod]
        public void TimeOutAtomicJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                // Time must be longer than the time-out of quick queue!
                var guid = ScheduleTestJob(new TimeSpan(0, 1, 0), JobType.AtomicDelay, QueueType.Quick);

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                Assert.IsTrue(ji.JobExecutionStatus == JobExecutionState.TimedOut ||
                    ji.JobExecutionStatus == JobExecutionState.Completed);
            }
        }

        [TestMethod]
        public void PersistAndResumeJobTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                // Time must be longer than the time-out of quick queue!
                var guid = ScheduleTestJob(new TimeSpan(0, 0, 20), JobType.MultipleDelay, QueueType.Long);

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                // Suspend workflows
                SchedulerTester.Instance.Stop();

                // Leave enough time to suspend
                Thread.Sleep(new TimeSpan(0, 0, 30));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Persisted, ji.JobExecutionStatus);

                SchedulerTester.Instance.EnsureRunning();

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                Assert.AreEqual("OK", (string)ji.Parameters["Result"].GetValue());
            }
        }

        [TestMethod]
        public void PersistAndCancelTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                // Time must be longer than the time-out of quick queue!
                var guid = ScheduleTestJob(new TimeSpan(0, 0, 20), JobType.MultipleDelay, QueueType.Long);

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                // Suspend workflows
                SchedulerTester.Instance.Stop();

                // Leave enough time to suspend
                Thread.Sleep(new TimeSpan(0, 0, 30));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Persisted, ji.JobExecutionStatus);

                using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                {
                    ji.Context = context;
                    ji.Cancel();
                }

                ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Persisted | JobExecutionState.CancelRequested, ji.JobExecutionStatus);

                SchedulerTester.Instance.EnsureRunning();

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
            }
        }
    }
}
