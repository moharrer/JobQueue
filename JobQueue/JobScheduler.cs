using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace JobQueue
{
    public class JobScheduler
    {
        private static Timer Timer = new Timer();
        static Action<Job> execute = (job) => { };
        static JobScheduler()
        {
            Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            Timer.Interval = 5000;

        }
        public static void Start(Action<Job> executeFunc)
        {
            execute = executeFunc;

            var jobs = FakeHttpService.GetJobs();

            foreach (var container in jobs)
            {
                var processor = JobProccessorFactory.GetProccessor(container.Job.Category, execute);
                processor.Enqueue(container);
            }

            if (Configuration.TimerEnable)
                Timer.Enabled = true;
        }
        public static void Stop()
        {
            Timer.Enabled = false;

            var express = JobProccessorFactory.GetProccessor(JobCategory.Express, execute);
            var normal = JobProccessorFactory.GetProccessor(JobCategory.Normal, execute);

            express.Stop();
            normal.Stop();
        }


        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("*********************** Http call ************************");

            var jobs = FakeHttpService.GetJobs();
            foreach (var container in jobs)
            {
                var processor = JobProccessorFactory.GetProccessor(container.Job.Category, execute);
                processor.Enqueue(container);
            }

        }
    }
}
