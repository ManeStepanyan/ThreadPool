using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPool
{
    public class NewThreadPool : IDisposable
    {
        private Object locker = new object();
        private EventWaitHandle autoResetEvent = new AutoResetEvent(false); // to notify other thread about an occured event
        private Queue<Thread> threads; // working threads
        private Queue<Action> tasks = new Queue<Action>(); // tasks to execute
        private readonly int maxsize; // maximum number of working threads in thread pool
        private bool isDisposed; // to indicate whether Dispose() is called
        /// <summary>
        /// Initialize new thread pool
        /// </summary>
        /// <param name="maxsize"> maximum number of working threads in thread pool </param>
        public NewThreadPool(int maxsize)
        {
            threads = new Queue<Thread>();
            this.maxsize = maxsize;
            for (int i = 0; i < maxsize; i++)
            {
                var thread = new Thread(Work) { Name = $"Thread number {i + 1}" };
                thread.Start();
                lock (locker)
                {
                    threads.Enqueue(thread);
                }
            }
        }
        /// <summary>
        /// Add new task to thread pool
        /// </summary>
        /// <param name="action"> task to be done </param>
        public void AddTask(Action action)
        {
            if (isDisposed) throw new Exception("Can not add anymore ");
            lock (locker)
            {
                tasks.Enqueue(action);
            }
            autoResetEvent.Set();
        }
        /// <summary>
        /// release unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                { // manual release of managed resources
                  // it is possible that disposed is called when some tasks haven't finished yet. So we have to wait them to complete their work.                        
                    while (this.tasks.Count > 0)
                    {
                        autoResetEvent.WaitOne();
                    }
                }
                autoResetEvent.Close();
                isDisposed = true;
            }
        }
        ~NewThreadPool()
        {
            Dispose(false);
        }
        /// <summary>
        /// Executing task
        /// </summary>
        public void Work()
        {
            Action task = null;
            while (true)
            {
                while (true)
                {
                    if (isDisposed) return;

                    if (tasks.Count() > 0 && threads.Count != 0)
                    {
                        lock (locker)
                        {
                            task = tasks.Dequeue();
                            threads.Dequeue();
                        }

                        autoResetEvent.Set();
                        break;
                    }
                    // otherwise we should wait till new task or thread to be freed
                    autoResetEvent.WaitOne();
                }
                if (task != null)
                    task();
                // as task is completed, we have to add a thread to the queue, as it can perform another work
                lock (locker)
                {
                    threads.Enqueue(Thread.CurrentThread);
                }

            }


        }
    }
}


