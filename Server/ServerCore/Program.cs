using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
        
    class Program
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>();

        static void WhoAmI()
        {
            ThreadName.Value = $"My Name Is {Thread.CurrentThread.ManagedThreadId}";

            Thread.Sleep(1000);

            Console.WriteLine(ThreadName.Value);
        }
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);

        }
    }
}
