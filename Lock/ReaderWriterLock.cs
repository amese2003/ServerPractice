using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerRecap
{
    // 재귀적 락을 허용할지 (YES) WriteLock -> WriteLock, WriteLock -> ReadLock 가능 ReadLock->WriteLock만 불가능
    // 스핀락 5000번 -> yield

    class T_ReaderWriterLock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        // [Unuesd(1)] [WirteThreadId(15))] [ReadCount(16)]
        int _flag = EMPTY_FLAG;
        int _writeCount = 0;

        public void WriteLock()
        {
            // 동일 스레드가 WriteLock을 이미 가지고 있는가?
            int lockThreadId = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                _writeCount++;
                return;
            }


            // 아무도 writeLock이나 readLock을 가지고 있지 않으면 휙득
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while (true)
            {
                for(int i = 0; i <MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }

                Thread.Yield();
            }
        }

        public void WriteExit()
        {
            int lockCount = --_writeCount;
            if (lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);

            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 동일 스레드가 WriteLock을 이미 가지고 있는가?
            int lockThreadId = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            // writeLock 소유권 확인
            while (true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) // A (0 => 1) B (0 => 1)
                        return;
                }

                Thread.Yield();
            }
        }

        public void ReadExit()
        {
            Interlocked.Decrement(ref _flag);
        }
    }


    class RProgram
    {
        static volatile int count = 0;
        static T_ReaderWriterLock _lock = new T_ReaderWriterLock();

        static void Main(string[] args)
        {
            Task t1 = new Task(delegate () { 
            
                for(int i = 0; i < 100000; i++)
                {
                    _lock.WriteLock();
                    count++;
                    _lock.WriteExit();
                }
            
            });

            Task t2 = new Task(delegate () {

                for (int i = 0; i < 100000; i++)
                {
                    _lock.WriteLock();
                    count++;
                    _lock.WriteExit();
                }

            });


            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);
            Console.WriteLine(count);
        }
    }
}
