using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
        
    class Program
    {
        class Reward
        {

        }

        // 상호배제
        static object _lock = new object(); // 근성
        static SpinLock _lock2 = new SpinLock(); // 근성 + 양보
        //static Mutex _lock3 = new Mutex(); // 운영체제 갑질

        //RWLock ReaderWriteLock
        static ReaderWriterLockSlim _lock3 = new ReaderWriterLockSlim();

        // 99.9999....
        static Reward GetRewardById(int id)
        {
            _lock3.EnterReadLock();

            _lock3.ExitReadLock();
            lock (_lock)
            {

            }
            return null;
        }

        //0.000001
        static void AddReward(Reward reward)
        {
            _lock3.EnterWriteLock();

            _lock3.ExitWriteLock();

            lock (_lock)
            {

            }
        }

        static void Main(string[] args)
        {
            lock (_lock)
            {

            }
        }
    }
}
