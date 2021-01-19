using System;
using System.Threading;

namespace JobQueue
{
    public static class Log
    {
        public static void Info(this JobContainer job, string text)
        {
            ColorConsole.WriteInfo(JobTemplate(job) + "\"" + text + "\"");
        }

        public static void Error(this JobContainer job, string text)
        {
            ColorConsole.WriteError("\"" + text + "\"" + Environment.NewLine +  JobTemplate(job));
        }

        public static void Warning(this JobContainer job, string text)
        {
            ColorConsole.WriteWarning("\"" + text + "\"" + Environment.NewLine + JobTemplate(job));
        }

        public static void WriteSuccess(this JobContainer job, string text)
        {
            ColorConsole.WriteSuccess("\"" + text + "\"" + Environment.NewLine + JobTemplate(job));
        }

        public static void Debuge(this JobContainer job, string text)
        {
            if (Configuration.IsLogDebuge)
                ColorConsole.WriteInfo("\"" + text + "\"" + Environment.NewLine + JobTemplate(job));
        }

        private static string JobTemplate(JobContainer jobContainer)
        {
            var j = jobContainer.Job;
            return $"   {Thread.CurrentThread.ManagedThreadId} {j.Category} {j.MessageId} {j.CreatedTimeUtc} {j.Entity} {j.PayLoad}, status: {jobContainer.Succeeded} isRun:{jobContainer.IsRunning} retry:{jobContainer.RetriedCount}, ";
        }
    }
}
