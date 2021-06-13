using System;
using System.Threading.Tasks;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using RabbitMQ.Client.Events;

namespace Farseer.Net.MQ.RabbitDemo.Consumer
{
    [Consumer(Name = "default", ExchangeName = "DeadLetter", QueueName = "DeadLetter", ExchangeType = eumExchangeType.fanout)]
    public class DeadLetterConsumer : IListenerMessage
    {
        public Task<bool> Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            System.Console.WriteLine(ea.ConsumerTag + "死信消息:" + message);
            return Task.FromResult(true);
        }

        public Task<bool> FailureHandling(string message, object sender, BasicDeliverEventArgs ea) => throw new NotImplementedException();
    }
}