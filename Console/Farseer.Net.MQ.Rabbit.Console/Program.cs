using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using FS;
using FS.DI;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using FS.Utils.Common;
using RabbitMQ.Client;

namespace Farseer.Net.MQ.Rabbit.Console
{
    [Rabbit]
    class Program
    {
        private static string queueName = "aaaa";

        static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            Task.Run(SendMessage);
            Thread.Sleep(-1);
        }

        private static void SendMessage()
        {
            Parallel.For(0, 10000000, new ParallelOptions {MaxDegreeOfParallelism = 8}, i =>
            {
                var message = $"PID:{Process.GetCurrentProcess().Id} index:{i},Time：{DateTime.Now}";
                IocManager.Instance.Resolve<IRabbitManager>("Test").Product.Send(message, o =>
                {
                    o.Expiration = "1000";
                });
            });
        }
    }
}