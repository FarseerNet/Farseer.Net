using System;
using System.Threading.Tasks;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using RabbitMQ.Client.Events;

namespace Farseer.Net.MQ.RabbitDemo.Consumer
{
    /// <summary>
    /// 消费客户端
    /// </summary>
    [Consumer(Enable = false, Name = "default", ExchangeName = "test", QueueName = "test", ExchangeType = eumExchangeType.fanout, DlxExchangeName = "DeadLetter")]
    public class TestConsumer : IListenerMessage
    {
        public Task<bool> Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            System.Console.WriteLine(ea.ConsumerTag + "接收到信息为:" + message);
            return Task.FromResult(true);
        }

        public Task<bool> FailureHandling(string message, object sender, BasicDeliverEventArgs ea) => throw new NotImplementedException();
    }
}