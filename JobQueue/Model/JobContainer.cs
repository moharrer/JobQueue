namespace JobQueue
{
    public class JobContainer
    {
        public Job Job { get; private set; }
        public int RetriedCount { get; private set; }

        //Job finished successfully
        public bool? Succeeded { get; private set; }
        public bool IsRunning { get; set; }
        public string DependentJobId;
        public JobContainer() { }
        public JobContainer(Job job)
        {
            this.Job = job;
            this.RetriedCount = 0;
        }

        public void IncreaseRetryCount()
        {
            RetriedCount++;
        }
        public void MarkAsSuceeded()
        {
            this.Succeeded = true;
            this.IsRunning = false;
        }

        public void MarkAsFailed()
        {
            this.Succeeded = false;
            this.IsRunning = false;
        }

        public bool IsDependOnJob()
        {
            return string.IsNullOrEmpty(DependentJobId) == false;
        }
    }

}
