using System;
using System.Threading;

namespace JobQueue
{
    public class WorkerThread<T> where T : class, new()
    {
        private readonly JobQueueProccessor queue;
        ManualResetEvent mrse ;
        private ManualResetEvent abortEvent;
        private Thread thread;

        public WorkerThread(JobQueueProccessor queue)
        {
            this.queue = queue;
            abortEvent = new ManualResetEvent(false);
            mrse = new ManualResetEvent(false);
            thread = new Thread(RunJob);
        }

        public bool IsSignaled
        {
            get { return mrse.WaitOne(0); }
        }

        public void Start()
        {
            thread.Start();
        }

        public void Stop()
        {
            abortEvent.Set();
            //ensure thread is completed before stop
            thread.Join();
        }

        public void Resume()
        {
            mrse.Set();
        }

        public void Pause()
        {
            mrse.Reset();
        }

        private void RunJob()
        {
            WaitHandle[] handles = new WaitHandle[] { mrse, abortEvent };

            while (true)
            {
                switch (WaitHandle.WaitAny(handles))
                {
                    case 0:
                        {
                            ProcessItems();
                        }
                        break;
                    case 1:
                        {
                            return;
                        }
                }
                
            }
        }

        private void ProcessItems()
        {
            while (queue.TryDequeue(out JobContainer job))
            {
                if (queue.JobCanProccess(job))
                {
                    queue.Execute(job);
                }
                               
                if (!mrse.WaitOne(0) || abortEvent.WaitOne(0)) return;
            }

            ColorConsole.WriteLine($"Thread  {Thread.CurrentThread.ManagedThreadId} paused.");
            mrse.Reset();
        }
    }
}
