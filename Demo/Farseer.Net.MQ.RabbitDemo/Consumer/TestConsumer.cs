using System;
using System.Threading.Tasks;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using RabbitMQ.Client.Events;

namespace Farseer.Net.MQ.RabbitDemo.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = false, Server = "default", ExchangeName = "test", QueueName = "test", ExchangeType = eumExchangeType.fanout, DlxExchangeName = "DeadLetter")]
    public class TestConsumer : IListenerMessage
    {
        public Task<bool> Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            Console.WriteLine(value: ea.ConsumerTag + "接收到信息为:" + message);
            return Task.FromResult(result: true);
        }
    }
}