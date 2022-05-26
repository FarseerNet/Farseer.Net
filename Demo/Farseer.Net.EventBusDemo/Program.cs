using System;
using System.Diagnostics;
using FS;
using FS.Core.Abstract.EventBus;
using FS.DI;

namespace Farseer.Net.EventBusDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // 项目启动时初始化
            FarseerApplication.Run<StartupModule>().Initialize();

            // ******************** 以下演示消息发送 *********************
            
            // 先执行jit
            var eventProduct = IocManager.GetService<IEventProduct>(name: "test");
            eventProduct.SendSync(null, message: "测试发送消息内容");
            // 开启时间测试
            var startNew = Stopwatch.StartNew();

            // ******************** 测试1秒内，能发送多少条消息 *********************
            
            // 同步发送：每秒秒发送42W条事件消息
            SyncSend(startNew: startNew, eventProduct: eventProduct);
            // 异步发送：每秒秒发送95W条事件消息
            ASyncSend(startNew: startNew, eventProduct: eventProduct);
        }
        
        private static void SyncSend(Stopwatch startNew, IEventProduct eventProduct)
        {

            var count = 0;
            startNew.Reset();
            startNew.Restart();
            while (startNew.ElapsedMilliseconds < 1000)
            {
                eventProduct.SendSync(null, message: DateTime.Now);
                count++;
            }

            Console.WriteLine(value: $"同步共发送了：{count:N0}/秒 消息");
        }
        
        private static void ASyncSend(Stopwatch startNew, IEventProduct eventProduct)
        {

            var count = 0;
            startNew.Reset();
            startNew.Restart();
            while (startNew.ElapsedMilliseconds < 1000)
            {
                eventProduct.SendAsync(null, message: DateTime.Now);
                count++;
            }

            Console.WriteLine(value: $"异步共发送了：{count:N0}/秒 消息");
        }
    }
}