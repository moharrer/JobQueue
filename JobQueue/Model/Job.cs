using System;

namespace JobQueue
{
    public class Job    
    {
        public Job(){}

        public Job(string messageId, JobCategory category, string entity, string payload, DateTime createdTimeUtc)
        {
            this.MessageId = messageId;
            this.Category = category;
            this.Entity = entity;
            this.PayLoad = payload;
            this.CreatedTimeUtc = createdTimeUtc;
        }
        public DateTime CreatedTimeUtc { get; set; }
        public string MessageId { get; set; }
        public string Entity { get; set; }
        public JobCategory Category { get; set; }
        public string PayLoad { get; set; }
    }
}
