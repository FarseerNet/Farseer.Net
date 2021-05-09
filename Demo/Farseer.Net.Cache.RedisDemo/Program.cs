using FS;

namespace Farseer.Net.Cache.RedisDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
        }
    }
}