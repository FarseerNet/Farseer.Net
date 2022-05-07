using System;
using System.Diagnostics;
using System.Threading;
using FS;
using FS.DI;
using FS.MQ.Queue;
using FS.MQ.Queue.Attr;

namespace Farseer.Net.MQ.QueueDemo
{
    [Queue]
    internal class Program
    {
        private static void Main(string[] args)
        {
            // 项目启动时初始化
            FarseerApplication.Run<StartupModule>().Initialize();

            // ******************** 以下演示消息发送 *********************
            var rabbitProduct = IocManager.GetService<IQueueManager>(name: "test").Product;
            // 先执行jit
            rabbitProduct.Send("测试发送消息内容");
            // 开启时间测试
            var startNew = Stopwatch.StartNew();
            // 以上也是JIT

            // ******************** 测试1秒内，能发送多少条消息 *********************
            Send(startNew: startNew, rabbitProduct: rabbitProduct);
            Thread.Sleep(millisecondsTimeout: -1);
        }
        private static void Send(Stopwatch startNew, IQueueProduct rabbitProduct)
        {
            var count = 0;
            startNew.Reset();
            startNew.Restart();
            while (startNew.ElapsedMilliseconds < 1000)
            {
                rabbitProduct.Send(DateTime.Now.ToString());
                count++;
            }

            Console.WriteLine(value: $"共发送了：{count:N0}/秒 消息");
        }
    }
}