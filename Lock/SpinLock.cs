using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerRecap
{
    class SpinLock
    {
        volatile int _locked = 0;

        public void Acquire_Exchange()
        {
            while (true)
            {
                // 인터락 방식으로..
                int original = Interlocked.Exchange(ref _locked, 1);
                if (original == 0)
                    break;
            }
        }

        public void Acquire_CAS()
        {
            while (true)
            {
                int expected = 0;
                int desired = 1;

                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                //Thread.Sleep(1); // 무조건 휴식 => 무조건 1ms 휴식
                //Thread.Sleep(0); // 조건부 휴식 => 나보다 우선순위가 낮으면 양보 불가 == 나보다 우선순위가 같거나 높은 쓰레드가 없으면 다시 본인한테
                Thread.Yield();  // 관대한 양보 => 관대하게 양보하니, 지금 실행 가능한 쓰래드가
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    class TProgram
    {
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for(int i = 0; i < 100000; i++)
            {
                _lock.Acquire_Exchange();
                _num++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for(int i = 0; i < 100000; i++)
            {
                _lock.Acquire_Exchange();
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
