using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerRecap
{
    class EventLock
    {
        //AutoResetEvent _available = new AutoResetEvent(true);
        ManualResetEvent _available = new ManualResetEvent(false);

        public void Acquire()
        {
            _available.WaitOne();
        }

        public void Release()
        {
            _available.Set();
        }
    }

    class AutoResetEvent
    {
        static int _num = 0;
        static EventLock _lock = new EventLock();

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}
