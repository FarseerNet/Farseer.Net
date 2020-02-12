using System;
using FS;

namespace Farseer.Net.Cache.Redis.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            FarseerBootstrapper.Create<StartupModule>().Initialize();
        }
    }
}