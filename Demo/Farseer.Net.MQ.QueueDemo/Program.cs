using System;
using System.Diagnostics;
using System.Threading;
using FS;
using FS.Core.LinkTrack;
using FS.DI;
using FS.LinkTrack;
using FS.MQ.Queue;
using FS.MQ.Queue.Attr;

namespace Farseer.Net.MQ.QueueDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // 项目启动时初始化
            FarseerApplication.Run<StartupModule>().Initialize();

            // ******************** 以下演示消息发送 *********************
            var queueProduct = IocManager.GetService<IQueueManager>(name: "test").Product;
            // 先执行jit
            queueProduct.Send("测试发送消息内容");
            // 开启时间测试
            var startNew = Stopwatch.StartNew();
            // 以上也是JIT

            // ******************** 测试1秒内，能发送多少条消息 *********************
            // 每秒发送：2,041,833条数据
            Send(startNew: startNew, rabbitProduct: queueProduct);
            Thread.Sleep(millisecondsTimeout: -1);
        }
        private static void Send(Stopwatch startNew, IQueueProduct rabbitProduct)
        {
            var count = 0;
            startNew.Reset();
            startNew.Restart();
            var linkTrackQueue = new LinkTrackQueue();
            while (startNew.ElapsedMilliseconds < 1000)
            {
                linkTrackQueue.Enqueue(new LinkTrackContext());
                //rabbitProduct.Send(DateTime.Now.ToString());
                count++;
            }

            Console.WriteLine(value: $"共发送了：{count:N0}/秒 消息");
        }
    }
}