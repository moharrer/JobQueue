using System;
using System.Threading;

namespace JobQueue
{
    public class JobExecutor
    {
        public void Execute(Job job)
        {
            //job1 failed inorder to testing job retry 
            if (job.PayLoad == "payload: job1")
            {
                throw new Exception();
            }

            Thread.Sleep(1000);
        }

    }
}
