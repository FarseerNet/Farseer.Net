using System;
using System.Collections.Generic;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using RabbitMQ.Client.Events;

namespace Farseer.Net.MQ.Rabbit.Console.Consumer
{
    [Consumer(Enable = false, Name = "default", ExchangeName = "test", QueueName = "test", ExchangeType = eumExchangeType.direct, DlxExchangeName = "DeadLetter")]
    public class TestConsumer : IListenerMessage
    {
        public bool Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            System.Console.WriteLine(ea.ConsumerTag + "接收到信息为:" + message);
            return true;
        }

        public bool FailureHandling(string message, object sender, BasicDeliverEventArgs ea) => throw new NotImplementedException();
    }
}