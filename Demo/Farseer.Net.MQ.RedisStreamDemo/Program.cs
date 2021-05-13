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
    class Program
    {
        static void Main(string[] args)
        {
            // 项目启动时初始化
            FarseerApplication.Run<StartupModule>().Initialize();

            // ******************** 以下演示消息发送 *********************
            var redisStreamProduct = IocManager.Instance.Resolve<IRedisStreamProduct>("test");
            redisStreamProduct.Send(DateTime.Now).ToString();

            // ******************** 以下为了准备测试1S内，能发多少条消息 *********************
            var startNew = Stopwatch.StartNew();
            startNew.Start();
            Thread.Sleep(1000);
            startNew.Stop();
            // 以上为了JIT，以防测试差异较大

            // ******************** 测试1秒内，能发送多少条消息 *********************
            var count = 0;
            startNew.Reset();
            startNew.Restart();
            while (startNew.ElapsedMilliseconds < 1000)
            {
                redisStreamProduct.Send(DateTime.Now.ToString());
                count++;
            }

            Console.WriteLine(count);

            Thread.Sleep(-1);
        }
    }
}