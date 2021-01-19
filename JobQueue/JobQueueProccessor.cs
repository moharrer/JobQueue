using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JobQueue
{
    public partial class JobQueueProccessor
    {
        private object lockObject = new object();

        List<WorkerThread<Job>> WorkingThreadList = new List<WorkerThread<Job>>();
        private ConcurrentBag<JobContainer> AllJobs = new ConcurrentBag<JobContainer>();
        private ConcurrentQueue<JobContainer> JobQueue = new ConcurrentQueue<JobContainer>();

        public string Category { get; private set; }
        private readonly int ThreadCount;
        private readonly int retryCount;

        private Action<Job> JobeExcutor;

        #region Constructor
        public JobQueueProccessor(string category, int threadCount, Action<Job> action)
            : this(category, threadCount, Configuration.RetryCount, action)
        {

        }
        public JobQueueProccessor(string category, int threadCount, int retryCount, Action<Job> action)
        {

            this.Category = category;
            //minimum thread count should be 1
            this.ThreadCount = threadCount > 0 ? threadCount : 1;
            this.retryCount = retryCount > 0 ? retryCount : 1;
            JobeExcutor = action;

            CreateThread();
        }
        #endregion

        #region Methods
        public void Start()
        {
            lock (lockObject)
            {
                for (int i = 0; i < Math.Min(WorkingThreadList.Count, JobQueue.Count); i++)
                {
                    WorkingThreadList[i].Resume();
                }

            }
        }

        public void Stop()
        {
            lock (lockObject)
            {
                foreach (var thread in WorkingThreadList)
                {
                    //waiting for each thread to being finished
                    thread.Stop();
                }
            }
        }

        public void Enqueue(JobContainer job)
        {
            lock (lockObject)
            {
                AllJobs.Add(job);
                InternalEnqueue(job, true);
            }
        }

        public bool TryDequeue(out JobContainer job)
        {
            return JobQueue.TryDequeue(out job);
        }

        public bool JobCanProccess(JobContainer jobContainer)
        {
            if (IsParentProcessFailed(jobContainer))
            {
                jobContainer.MarkAsFailed();
                SetJobStopRunning(jobContainer);

                jobContainer.Error("failed because of parent job has been failed");

                return false;
            }
            if (CanJobProccess(jobContainer, out bool waiting) == false)
            {
                //it need to requeue to meet dependencies
                InternalEnqueue(jobContainer);
                return false;
            }

            SetJobInProccessing(jobContainer);
            return true;
        }

        public void Execute(JobContainer jobContainer)
        {
            try
            {
                jobContainer.Debuge("Start executing");

                //Execute job
                this.JobeExcutor(jobContainer.Job);

                jobContainer.MarkAsSuceeded();

                jobContainer.WriteSuccess("Finish executing Job");
            }
            catch
            {
                if (CanRetry(jobContainer) == false)
                {
                    jobContainer.MarkAsFailed();
                    jobContainer.Error($"failed after {jobContainer.RetriedCount} time retry");
                }

                SetJobStopRunning(jobContainer);

                if (jobContainer.Succeeded.HasValue == false || jobContainer.Succeeded.Value)
                {    //increase retry count
                    jobContainer.IncreaseRetryCount();

                    jobContainer.Warning("failed and retry after couple of seconds.");

                    //requeue
                    InternalEnqueue(jobContainer);

                }
            }

        }

        #endregion

        #region Utillity

        private void CreateThread()
        {
            lock (lockObject)
            {
                for (int i = 0; i < ThreadCount; i++)
                {
                    var thread = new WorkerThread<Job>(this);
                    this.WorkingThreadList.Add(thread);
                    thread.Start();
                }
            }
        }

        private bool CanJobProccess(JobContainer container, out bool waiting)
        {
            waiting = false;

            var eee = AllJobs.Where(a => a.Job.Entity == container.Job.Entity && a.IsRunning).ToList();

            //Check if a job with this 'entityName' already running
            var runningJob = AllJobs.FirstOrDefault(a => a.Job.Entity == container.Job.Entity &&
                                                 a.IsRunning);
            if (runningJob != null)
            {
                container.Debuge($"Requeue: this job is waiting for Job {runningJob.Job.MessageId}.");
                waiting = true;
                return false;
            }
            //Check if this task is depend on other and dependent task exist in queue
            var hasParentJob = AllJobs.Any(a => a.Job.MessageId == container.DependentJobId);
            if (!string.IsNullOrEmpty(container.DependentJobId) && (hasParentJob == false))
            {
                container.Debuge($"Requeue job: This job is depend on job {container.DependentJobId}.");
                waiting = true;
                return false;
            }

            if (AllJobs.Any(a => a.Job.MessageId == container.DependentJobId && (!a.Succeeded.HasValue || !a.Succeeded.Value)))
            {
                container.Debuge($"Requeue job: This job is depend On job {container.DependentJobId} to be finished.");
                waiting = true;
                return false;
            }

            //check if waiting proccess exist 
            var waitingJob = AllJobs.Where(a => !a.IsRunning &&
                                 (!a.Succeeded.HasValue) && // job must not be finished
                                 a.Job.Entity == container.Job.Entity &&
                                 a.Job.CreatedTimeUtc < container.Job.CreatedTimeUtc)
                .OrderBy(a => a.Job.CreatedTimeUtc).FirstOrDefault();
            if (waitingJob != null)
            {
                //container.Debuge($"Requeue job: This job is waiting for job {waitingJob.Job.MessageId} to be finished.");

                return false;
            }

            return true;
        }

        private bool IsParentProcessFailed(JobContainer container)
        {
            if (AllJobs.Any(a => a.Job.MessageId == container.DependentJobId &&
                                 a.RetriedCount >= retryCount &&
                                (a.Succeeded.HasValue && a.Succeeded == false)))
            {
                return true;
            }
            return false;
        }

        private bool CanRetry(JobContainer container)
        {
            return container.RetriedCount < retryCount;
        }

        private void SetJobInProccessing(JobContainer jobContainer)
        {
            var mms = AllJobs.Single(a => a.Job.MessageId == jobContainer.Job.MessageId);
            mms.IsRunning = true;
        }
        private void SetJobStopRunning(JobContainer jobContainer)
        {
            var mms = AllJobs.Single(a => a.Job.MessageId == jobContainer.Job.MessageId);
            mms.IsRunning = false;
        }

        private void ResumeWaitingThreads()
        {
            var notRunningTask = WorkingThreadList.FirstOrDefault(a => a.IsSignaled == false);
            if (notRunningTask != null) notRunningTask.Resume();
        }

        private void InternalEnqueue(JobContainer jobContainer, bool resumeWaiting = false)
        {
            JobQueue.Enqueue(jobContainer);

            if (resumeWaiting)
                ResumeWaitingThreads();
        }

        #endregion
    }
}
