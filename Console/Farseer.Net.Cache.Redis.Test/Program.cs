using System;
using FS;

namespace Farseer.Net.Cache.Redis.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            FarseerBoot.Run<StartupModule>().Initialize();
        }
    }
}