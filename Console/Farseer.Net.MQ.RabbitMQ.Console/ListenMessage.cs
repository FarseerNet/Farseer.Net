using System;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using RabbitMQ.Client.Events;

namespace Farseer.Net.MQ.RabbitMQ.Console
{
    [Consumer(Name = "default", ExchangeName = "test",QueueName = "aaaa1", ExchangeType = eumExchangeType.direct)]
    public class ListenMessage : IListenerMessage
    {
        public bool Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            System.Console.WriteLine(ea.ConsumerTag + "接收到信息为:" + message);
            return true;
        }

        public bool FailureHandling(string message, object sender, BasicDeliverEventArgs ea) => throw new NotImplementedException();
    }
}