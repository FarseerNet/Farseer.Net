using System;
using System.Threading.Tasks;
using FS.MQ.RedisStream;
using FS.MQ.RedisStream.Attr;
using StackExchange.Redis;

namespace Farseer.Net.MQ.RedisStreamDemo.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = true, RedisName = "default", GroupName = "", QueueName = "test2", PullCount = 2, ConsumeThreadNums = 1)]
    public class TestConsumer : IListenerMessage
    {
        public Task<bool> Consumer(StreamEntry[] messages, ConsumeContext ea)
        {
            foreach (var message in messages)
            {
                Console.WriteLine(value: "接收到信息为:" + message.Values[0]);
                ea.Ack(message: message);
            }

            return Task.FromResult(result: true);
        }

        public Task<bool> FailureHandling(StreamEntry[] messages, ConsumeContext ea) => throw new NotImplementedException();
    }
}