using System;
using System.Collections.Generic;
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
            var lst               = new List<Task>();
            var redisCacheManager = IocManager.Instance.Resolve<IRedisCacheManager>();

            // 初始化
            await redisCacheManager.Db.HashSetAsync("test_async", "init", "");
            redisCacheManager.Db.HashSet("test_sync", "init", "");
            redisCacheManager.Db.KeyDelete("test_async");
            redisCacheManager.Db.KeyDelete("test_sync");

            var count = 10000;
            Console.WriteLine($"{count}条数据测试");
            var watch = new Stopwatch();
            
            watch = Stopwatch.StartNew();
            for (var index = 0; index < count; index++)
            {
                redisCacheManager.Db.HashSet("test_sync", index, "");
            }
            Console.WriteLine($"HashSet: {watch.ElapsedMilliseconds}ms");
                
            
            watch = Stopwatch.StartNew();
            for (var index = 0; index < count; index++)
            {
                await redisCacheManager.Db.HashSetAsync("test_async", index, "");
            }
            Console.WriteLine($"HashSetAsync: {watch.ElapsedMilliseconds}ms");


            watch = Stopwatch.StartNew();
            lst   = new List<Task>();
            for (var index = 0; index < count; index++)
            {
                lst.Add(HashSetAsync(index));
            }
            await Task.WhenAll(lst);
            Console.WriteLine($"HashSetAsyncByWhenAll: {watch.ElapsedMilliseconds}ms");
            
            
            watch = Stopwatch.StartNew();
            lst   = new List<Task>();
            var transaction = redisCacheManager.Db.CreateTransaction();
            for (var index = 0; index < count; index++)
            {
                lst.Add(transaction.HashSetAsync("test_transaction", index, ""));
            }
            await transaction.ExecuteAsync();
            await Task.WhenAll(lst);
            Console.WriteLine($"HashSetAsyncByTransaction: {watch.ElapsedMilliseconds}ms");
            
            
            watch = Stopwatch.StartNew();
            var batch = redisCacheManager.Db.CreateBatch();
            lst   = new List<Task>();
            for (var index = 0; index < count; index++)
            {
                lst.Add(batch.HashSetAsync("test_batch", index, ""));
            }
            batch.Execute();
            await Task.WhenAll(lst);
            Console.WriteLine($"HashSetAsyncByBatch: {watch.ElapsedMilliseconds}ms");
        }

        private static Task<bool> HashSetAsync(int index)
        {
            var a = 100;
            return IocManager.Instance.Resolve<IRedisCacheManager>().Db.HashSetAsync("test_task", index, "");
        }
    }
}