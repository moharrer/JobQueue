using System;

namespace JobQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            ColorConsole.WriteWarning(
@" *****************************************Senario*****************************************
   * job list is in JobCollection.cs and Http call simulation int FakeHttpService.cs
   * Express category: 
   * HttpCall occure after 5 seconds
   * Default is  5 thread and can be configurable
   * Failure retry count is 3 and be configurable
   * job1 must fail after 3 time retry
   * job 1 & job2 & job3 have same entity name then executing order is by creation time (job1, job2, job3)
   * job 1 is depend on job9 
   * job 2 is depend on job8
   * job 3 is depend on job9
   *****************************************************************************************"
            );

            ColorConsole.WriteLine("For stopping jobs press 'e'");
            ColorConsole.WriteLine("Press any key to Start jobs");
            Console.Read();

            var jobExecutor = new JobExecutor();
            JobScheduler.Start(jobExecutor.Execute);

            while (true)
            {
                var input = Console.ReadKey();
                if (input.KeyChar == 'e')
                {
                    JobScheduler.Stop();
                }
            }
        }



    }
}
