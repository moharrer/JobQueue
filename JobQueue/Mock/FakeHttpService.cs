using System.Collections.Generic;
using System.Linq;

namespace JobQueue
{
    public class FakeHttpService
    {
        public static IEnumerable<JobContainer> GetJobs()
        {
            var express = JobCollection.GetExpressJobs();
            var normal = JobCollection.GetNormalJobs();

            var jobs = new List<JobContainer>();
            jobs.AddRange(express.ToList());
            jobs.AddRange(normal);

            return jobs;
        }

    }
}
