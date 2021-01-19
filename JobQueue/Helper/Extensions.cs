using System;
using System.Collections.Generic;
using System.Text;

namespace JobQueue
{
    public static class Extensions
    {
        public static JobContainer ToJobContainer(this Job job, Job dependentJob = null)
        {
            var jc = new JobContainer(job);
            if (dependentJob != null)
                jc.DependentJobId = dependentJob.MessageId;

            return jc;
        }
    }
}
