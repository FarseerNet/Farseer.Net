using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.Core.Abstract.MQ.Rabbit;
using FS.Extends;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using RabbitMQ.Client.Events;

namespace Farseer.Net.MQ.Rabbit.Test.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = true, Server = "default", ExchangeName = "UnitTest", QueueName = "UnitTest", ExchangeType = eumExchangeType.fanout, DlxExchangeName = "DeadLetter")]
    public class UnitTestConsumer : IListenerMessage
    {
        public static long ID;

        public Task<bool> Consumer(string message, object sender, BasicDeliverEventArgs ea)
        {
            ID = message.ConvertType(0L);
            return Task.FromResult(true);
        }
        public Task<bool> FailureHandling(IEnumerable<object> messages) => throw new NotImplementedException();
    }
}