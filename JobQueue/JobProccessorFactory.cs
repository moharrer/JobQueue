using System;

namespace JobQueue
{
    public class JobProccessorFactory
    {
        static JobQueueProccessor jobQueueWorkerExpress = null;
        static JobQueueProccessor jobQueueWorkerNormal = null;
        public static JobQueueProccessor GetProccessor(JobCategory category, Action<Job> execute)
        {
            if (category == JobCategory.Express)
            {
                if (jobQueueWorkerExpress == null)
                {
                    jobQueueWorkerExpress = new JobQueueProccessor(JobCategory.Express.ToString(), Configuration.ExpressCategoryMaxThreadCount, execute);
                    jobQueueWorkerExpress.Start();
                }
                return jobQueueWorkerExpress;
            }
            else
            {
                if (jobQueueWorkerNormal == null)
                {
                    jobQueueWorkerNormal = new JobQueueProccessor(JobCategory.Normal.ToString(), Configuration.NormalCategoryMaxThreadCount, execute);
                    jobQueueWorkerNormal.Start();
                }
                return jobQueueWorkerExpress;
            }
        }
    }
}
