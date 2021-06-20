using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FS;
using FS.Cache.Redis;
using FS.DI;

namespace Farseer.Net.Cache.RedisDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();

            var redisCacheManager = IocManager.Instance.Resolve<IRedisCacheManager>();

            // 初始化
            await redisCacheManager.Db.HashSetAsync("test_async", "init", "");
            redisCacheManager.Db.HashSet("test_sync", "init", "");

            var watch = new Stopwatch();
            watch.Start();
            for (var index = 0; index < 10000; index++)
            {
                await redisCacheManager.Db.HashSetAsync("test_async", index, "");
            }

            watch.Stop();
            Console.WriteLine($"test_async: {watch.ElapsedMilliseconds}ms");

            watch = new Stopwatch();
            watch.Start();
            for (var index = 0; index < 10000; index++)
            {
                redisCacheManager.Db.HashSet("test_sync", index, "");
            }

            watch.Stop();
            Console.WriteLine($"test_async: {watch.ElapsedMilliseconds}ms");
        }
    }
}