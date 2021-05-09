using System;
using System.Diagnostics;
using System.Threading;
using FS;
using FS.DI;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;

namespace Farseer.Net.MQ.RabbitDemo
{
    [Rabbit]
    class Program
    {
        static void Main(string[] args)
        {
            // 项目启动时初始化
            FarseerApplication.Run<StartupModule>().Initialize();
            
            // ******************** 以下演示消息发送 *********************
            var rabbitProduct = IocManager.Instance.Resolve<IRabbitManager>("test").Product;
            
            // 先执行jit
            rabbitProduct.Send("测试发送消息内容");
            // 开启时间测试
            var startNew = Stopwatch.StartNew();
            startNew.Start();
            Thread.Sleep(1000);
            startNew.Stop();
            // 以上也是JIT
            
            
            // ******************** 测试1秒内，能发送多少条消息 *********************
            var count = 0;
            startNew.Reset();
            startNew.Restart();
            while (startNew.ElapsedMilliseconds < 1000)
            {
                rabbitProduct.Send(DateTime.Now.ToString());
                count++;
            }

            Console.WriteLine($"共发送了：{count} 消息");
            Thread.Sleep(-1);
        }
    }
}