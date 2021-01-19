using System;
using System.Collections.Generic;
using System.Text;

namespace JobQueue
{
    public partial class JobCollection
    {
        public static IEnumerable<JobContainer> GetExpressJobs()
        {
            var now = DateTime.UtcNow;
            var job1 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity1", payload: "payload: job1", now);
            var job2 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity1", payload: "payload: job2", now.AddSeconds(10));
            var job3 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity2", payload: "payload: job3", now.AddSeconds(15));
            var job4 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity3", payload: "payload: job4", now.AddSeconds(20));
            var job5 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity4", payload: "payload: job5", now.AddSeconds(25));
            var job6 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity5", payload: "payload: job6", now.AddSeconds(30));
            var job7 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity6", payload: "payload: job7", now.AddSeconds(35));
            var job8 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity2", payload: "payload: job8", now.AddSeconds(40));
            var job9 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity3", payload: "payload: job9", now.AddSeconds(45));
            var job10 = new Job(messageId: GetNewId(), category: JobCategory.Express, entity: "entity1", payload: "payload: job10", now.AddSeconds(50));

            //return new List<JobContainer>() { job1.ToJobContainer(job9), job9.ToJobContainer()};
            //return new List<JobContainer>() { job1, job2, job10 };
            //return new List<JobContainer>() { job1.ToJobContainer(job9), job2.ToJobContainer(job8) };
            return new List<JobContainer>() {
                job1.ToJobContainer(job9), job2.ToJobContainer(job8), 
                job3.ToJobContainer(job9),
                job4.ToJobContainer(), job5.ToJobContainer(), job6.ToJobContainer(),
                job7.ToJobContainer(job6), 
                job8.ToJobContainer(), job9.ToJobContainer(), job10.ToJobContainer()};
        }
        public static IEnumerable<JobContainer> GetNormalJobs()
        {
            var now = DateTime.UtcNow;
            var job1 = new Job(messageId: GetNewId(), category: JobCategory.Normal , entity: "entity10", payload: "payload: Normal 1 ", now);
            var job2 = new Job(messageId: GetNewId(), category: JobCategory.Normal, entity: "entity10", payload: "payload: Normal 2 ", now.AddSeconds(10));
            var job3 = new Job(messageId: GetNewId(), category: JobCategory.Normal, entity: "entity20", payload: "payload: Normal 3 ", now.AddSeconds(15));
            var job4 = new Job(messageId: GetNewId(), category: JobCategory.Normal, entity: "entity30", payload: "payload: Normal 4 ", now.AddSeconds(20));
            

            //return new List<Job>() { job1, job2};
            return new List<JobContainer>() { job1.ToJobContainer(), job2.ToJobContainer(), job3.ToJobContainer(), job4.ToJobContainer() };
        }

        private static string GetNewId()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}
