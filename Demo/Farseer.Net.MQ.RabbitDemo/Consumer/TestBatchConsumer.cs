using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using RabbitMQ.Client;

namespace Farseer.Net.MQ.RabbitDemo.Consumer
{
    /// <summary>
    ///     批量消费客户端
    /// </summary>
    [Consumer(Enable = true, Name = "default", ExchangeName = "test", QueueName = "test_batch", ExchangeType = eumExchangeType.fanout, PrefetchCountOrPullNums = 1000, BatchPullSleepTime = 200)]
    public class TestBatchConsumer : IListenerMessageBatch
    {
        public Task<bool> Consumer(List<string> messages, List<BasicGetResult> resp)
        {
            Console.WriteLine(value: $"接收到{messages.Count}条数据");
            return Task.FromResult(result: true);
        }
    }
}