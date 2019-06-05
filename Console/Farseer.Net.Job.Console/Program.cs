using System;
using FS;
using FS.Job;

namespace Farseer.Net.Job.Console
{
    public class Program
    {
        public static void Main()
        {
            FarseerBootstrapper.Create<StartupModule>().Initialize();
        }
    }
}