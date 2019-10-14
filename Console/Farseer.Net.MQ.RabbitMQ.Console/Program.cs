using System;
using System.Diagnostics;
using System.Text;
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
            FarseerBootstrapper.Create<StartupModule>().Initialize();

            IConnectionFactory conFactory = new ConnectionFactory //创建连接工厂对象
            {
                HostName = "mq.sz.cprapi.com", //IP地址
                Port = 5672, //端口号
                UserName = "steden", //用户账号
                Password = "steden" //用户密码
            };

            System.Console.WriteLine("请输入：1）发送；2）消费");
            switch (System.Console.ReadLine())
            {
                case "2":
                {
                    System.Console.Title = "消费";
                    Consumer(conFactory);
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
            IocManager.Instance.Resolve<IRabbitManager>().CreateQueue();
            IocManager.Instance.Resolve<IRabbitManager>().Product.Start();


            Parallel.For(0, 10000000, new ParallelOptions {MaxDegreeOfParallelism = 64}, i =>
            {
                var message = $"PID:{Process.GetCurrentProcess().Id} index:{i},Time：{DateTime.Now}";

                IocManager.Instance.Resolve<IRabbitManager>().Product.Send(message);
                System.Console.WriteLine(message);
            });
        }

        public static void Consumer(IConnectionFactory conFactory)
        {
            IocManager.Instance.Resolve<IRabbitManager>().Consumer.Start();
            IocManager.Instance.Resolve<IRabbitManager>().Consumer.Listener(new ListenMessage());
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