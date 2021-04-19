using System;
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

namespace Farseer.Net.MQ.RabbitMQ.Console
{
    [Rabbit]
    class Program
    {
        private static string queueName = "aaaa";

        static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            Thread.Sleep(-1);
            //SendMessage();
        }

        private static void SendMessage()
        {
            Parallel.For(0, 10000000, new ParallelOptions {MaxDegreeOfParallelism = 64}, i =>
            {
                var message = $"PID:{Process.GetCurrentProcess().Id} index:{i},Time：{DateTime.Now}";

                if (i%2 == 0) IocManager.Instance.Resolve<IRabbitManager>("test1").Product.Send(message, "");
                else IocManager.Instance.Resolve<IRabbitManager>("test1").Product.Send(message, "");
                System.Console.WriteLine(message);
            });
        }
    }
}