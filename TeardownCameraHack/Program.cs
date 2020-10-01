using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TeardownCameraHack
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Waiting for Teardown Performance Test...");
            var teardownProcess = (Process)null;
            while (teardownProcess == null)
            {
                teardownProcess = Process.GetProcessesByName("teardown-perftest").FirstOrDefault();
                Thread.Sleep(1000);
            }
            Console.Clear();
            new Hack(teardownProcess).Start();
        }
    }
}
