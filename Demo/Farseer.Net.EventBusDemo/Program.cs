using System;
using System.Diagnostics;
using System.Threading;
using FS;
using FS.DI;
using FS.EventBus;

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
            IocManager.GetService<IEventProduct>(name: "test").Send(null, message: "测试发送消息内容");
            // 开启时间测试
            var startNew = Stopwatch.StartNew();
            // 以上也是JIT

            // ******************** 测试1秒内，能发送多少条消息 *********************
            var count = 0;
            startNew.Reset();
            startNew.Restart();
            while (startNew.ElapsedMilliseconds < 1000)
            {
                IocManager.GetService<IEventProduct>(name: "test").SendAsync(null, message: DateTime.Now.ToString());
                count++;
            }

            Console.WriteLine(value: $"共发送了：{count} 消息");
        }
    }
}