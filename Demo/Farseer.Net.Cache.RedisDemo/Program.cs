using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FS;
using FS.Cache.Redis;
using FS.DI;

namespace Farseer.Net.Cache.RedisDemo
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            var lst               = new List<Task>();
            var redisCacheManager = IocManager.Instance.Resolve<IRedisCacheManager>();

            // 初始化
            await redisCacheManager.Db.HashSetAsync(key: "test_async", hashField: "init", value: "");
            redisCacheManager.Db.HashSet(key: "test_sync", hashField: "init", value: "");
            redisCacheManager.Db.KeyDelete(key: "test_async");
            redisCacheManager.Db.KeyDelete(key: "test_sync");

            var count = 10000;
            Console.WriteLine(value: $"{count}条数据测试");
            var watch = new Stopwatch();

            watch = Stopwatch.StartNew();
            for (var index = 0; index < count; index++) redisCacheManager.Db.HashSet(key: "test_sync", hashField: index, value: "");
            Console.WriteLine(value: $"HashSet: {watch.ElapsedMilliseconds}ms");


            watch = Stopwatch.StartNew();
            for (var index = 0; index < count; index++) await redisCacheManager.Db.HashSetAsync(key: "test_async", hashField: index, value: "");
            Console.WriteLine(value: $"HashSetAsync: {watch.ElapsedMilliseconds}ms");


            watch = Stopwatch.StartNew();
            lst   = new List<Task>();
            for (var index = 0; index < count; index++) lst.Add(item: HashSetAsync(index: index));
            await Task.WhenAll(tasks: lst);
            Console.WriteLine(value: $"HashSetAsyncByWhenAll: {watch.ElapsedMilliseconds}ms");


            watch = Stopwatch.StartNew();
            lst   = new List<Task>();
            var transaction = redisCacheManager.Db.CreateTransaction();
            for (var index = 0; index < count; index++) lst.Add(item: transaction.HashSetAsync(key: "test_transaction", hashField: index, value: ""));
            await transaction.ExecuteAsync();
            await Task.WhenAll(tasks: lst);
            Console.WriteLine(value: $"HashSetAsyncByTransaction: {watch.ElapsedMilliseconds}ms");


            watch = Stopwatch.StartNew();
            var batch = redisCacheManager.Db.CreateBatch();
            lst = new List<Task>();
            for (var index = 0; index < count; index++) lst.Add(item: batch.HashSetAsync(key: "test_batch", hashField: index, value: ""));
            batch.Execute();
            await Task.WhenAll(tasks: lst);
            Console.WriteLine(value: $"HashSetAsyncByBatch: {watch.ElapsedMilliseconds}ms");
        }

        private static Task<bool> HashSetAsync(int index)
        {
            var a = 100;
            return IocManager.Instance.Resolve<IRedisCacheManager>().Db.HashSetAsync(key: "test_task", hashField: index, value: "");
        }
    }
}