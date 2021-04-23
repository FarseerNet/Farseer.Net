using System;
using System.Threading;
using FS;
using FS.Job;

namespace Farseer.Net.Job.Console
{
    public class Program
    {
        public static void Main()
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            Thread.Sleep(-1);
        }
    }
}