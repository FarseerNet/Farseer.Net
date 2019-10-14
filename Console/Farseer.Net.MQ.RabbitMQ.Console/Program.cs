using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Farseer.Net.MQ.RabbitMQ.Console
{
    class Program
    {
        static void Main(string[] args)
        {
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
                    Consumer(conFactory); break;
                }
                case "1":
                {
                    System.Console.Title = "发送";
                    SendMessage(conFactory); break;
                }
            }
        }

        private static void SendMessage(IConnectionFactory conFactory)
        {

            using (IConnection con = conFactory.CreateConnection()) //创建连接对象
            {
                using (IModel channel = con.CreateModel()) //创建连接会话对象
                {
                    string queueName = "queue1";
                    //声明一个队列
//                    channel.QueueDeclare(
//                        queue: queueName, //消息队列名称
//                        durable: false, //是否持久化
//                        exclusive: false,
//                        autoDelete: false,
//                        arguments: null
//                    );

                    Parallel.For(0, 10000000, new ParallelOptions { MaxDegreeOfParallelism = 64 }, i =>
                    {
                        var message = $"PID:{Process.GetCurrentProcess().Id} index:{i},Time：{DateTime.Now}";
                        
                        //消息内容
                        var body = Encoding.UTF8.GetBytes(message);
                        //发送消息
                        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
                        System.Console.WriteLine(message);
                    });
                    
                }
            }
        }

        public static void Consumer(IConnectionFactory conFactory)
        {
            using (IConnection conn = conFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    string queueName = "queue1";
                    //声明一个队列
//                    channel.QueueDeclare(
//                        queue: queueName,//消息队列名称
//                        durable: false,//是否缓存
//                        exclusive: false,
//                        autoDelete: false,
//                        arguments: null
//                    );
                    //创建消费者对象
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        byte[] message = ea.Body;//接收到的消息
                        System.Console.WriteLine("接收到信息为:" + Encoding.UTF8.GetString(message));
                    };
                    
                    //消费者开启监听
                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    System.Console.ReadKey();
                }
            }
        }
    }
}