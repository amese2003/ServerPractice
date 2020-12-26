using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    // 재귀적 락을 허용할지...(No)
    // 스핀락 (5000번 => yield)
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        // [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
        int _flag = EMPTY_FLAG;

        public void WriteLock()
        {
            // 아무도 WriteLock or ReadLock을 휙득하지 않으면 소유권 휙득;
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while (true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    //시도 성공 return;
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                        return;
                }

                Thread.Yield();
            }
        }

        public void WriteExit()
        {
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 아무도 WriteLock을 휙득하지 않으면...
            while (true)
            {               

                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) 
                        ;
                }
                Thread.Yield();
            }            
        }

        public void ReadExit()
        {
            Interlocked.Decrement(ref _flag);
        }

    }
}
