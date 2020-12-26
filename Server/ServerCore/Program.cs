using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
        
    class Program
    {
        static object _lock = new object(); // 근성
        static SpinLock _lock2 = new SpinLock(); // 근성 + 양보
        static Mutex _lock3 = new Mutex(); // 운영체제 갑질

        static void Main(string[] args)
        {
            lock (_lock)
            {

            }

            bool lockTaken = false;

            try
            {
                _lock2.Enter(ref lockTaken);
            }
            finally
            {
                if (lockTaken)
                    _lock2.Exit();
            }


        }
    }
}
