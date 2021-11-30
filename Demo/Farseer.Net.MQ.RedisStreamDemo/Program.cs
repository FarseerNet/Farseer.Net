using System;
using System.Diagnostics;
using System.Threading;
using FS;
using FS.DI;
using FS.MQ.RedisStream;
using FS.MQ.RedisStream.Attr;

namespace Farseer.Net.MQ.RedisStreamDemo
{
    [RedisStream]
    internal class Program
    {
        private static void Main(string[] args)
        {
            // 项目启动时初始化
            FarseerApplication.Run<StartupModule>().Initialize();

            // ******************** 以下演示消息发送 *********************
            var redisStreamProduct = IocManager.GetService<IRedisStreamProduct>(name: "test2");
            redisStreamProduct.Send(entity: DateTime.Now);

            // ******************** 以下为了准备测试1S内，能发多少条消息 *********************
            var startNew = Stopwatch.StartNew();
            startNew.Start();
            Thread.Sleep(millisecondsTimeout: 1000);
            startNew.Stop();
            // 以上为了JIT，以防测试差异较大

            // ******************** 测试1秒内，能发送多少条消息 *********************
            Send(redisStreamProduct: redisStreamProduct);

            Thread.Sleep(millisecondsTimeout: -1);
        }

        private static void Send(IRedisStreamProduct redisStreamProduct)
        {
            var startNew = Stopwatch.StartNew();
            var count    = 0;
            startNew.Reset();
            startNew.Restart();
            while (startNew.ElapsedMilliseconds < 1000)
            {
                redisStreamProduct.Send(message: count.ToString());
                count++;
            }

            Console.WriteLine(value: count);
        }
    }
}