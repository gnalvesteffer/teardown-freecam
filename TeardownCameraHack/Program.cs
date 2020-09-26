﻿using System;
using System.Diagnostics;
using System.Linq;

namespace TeardownCameraHack
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            HackTeardown();
        }

        private static void HackTeardown()
        {
            var teardownProcess = Process.GetProcessesByName("teardown-perftest").FirstOrDefault();
            if (teardownProcess != null)
            {
                new TeardownHack(teardownProcess).Start();
            }
            else
            {
                Console.WriteLine("Could not find Teardown Performance Test.");
                Console.WriteLine("Run the game first, and then run this hack.");
                throw new Exception("Failed to find Teardown process.");
            }
        }
    }
}
