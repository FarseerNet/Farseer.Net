using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using FS;
using FS.DI;
using FS.MQ.RabbitMQ;
using FS.Utils.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Farseer.Net.MQ.RabbitMQ.Console
{
    class Program
    {
        private static string queueName = "aaaa";

        static void Main(string[] args)
        {
            FarseerBoot.Run<StartupModule>().Initialize();
            IocManager.Instance.Resolve<IRabbitManager>("test1").CreateQueue("aaaa1");
            IocManager.Instance.Resolve<IRabbitManager>("test1").CreateQueue("aaaa2");
            
            System.Console.WriteLine("请输入：1）发送；2）消费");
            switch (System.Console.ReadLine())
            {
                case "2":
                {
                    System.Console.Title = "消费";
                    Consumer();
                    break;
                }
                case "1":
                {
                    System.Console.Title = "发送";
                    SendMessage();
                    break;
                }
            }
        }

        private static void SendMessage()
        {
            //IocManager.Instance.Resolve<IRabbitManager>("test1").CreateQueue();

            Parallel.For(0, 10000000, new ParallelOptions {MaxDegreeOfParallelism = 64}, i =>
            {
                var message = $"PID:{Process.GetCurrentProcess().Id} index:{i},Time：{DateTime.Now}";

                if (i%2 == 0) IocManager.Instance.Resolve<IRabbitManager>("test1").Product.Send(message, "aaaa1");
                else IocManager.Instance.Resolve<IRabbitManager>("test1").Product.Send(message, "aaaa2");
                System.Console.WriteLine(message);
            });
        }

        public static void Consumer()
        {
            IocManager.Instance.Resolve<IRabbitManager>("aaaa1").Consumer.Start(new ListenMessage());
            //IocManager.Instance.Resolve<IRabbitManager>("aaaa2").Consumer.Start(new ListenMessage());
            Thread.Sleep(-1);
        }
    }

    public class ListenMessage : IListenerMessage
    {
        public bool Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            System.Console.WriteLine(ea.ConsumerTag + "接收到信息为:" + message);
            return true;
        }
    }
}