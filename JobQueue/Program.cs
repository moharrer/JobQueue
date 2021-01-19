using System;

namespace JobQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            ColorConsole.WriteWarning("For stopping threads press 'e'");
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
