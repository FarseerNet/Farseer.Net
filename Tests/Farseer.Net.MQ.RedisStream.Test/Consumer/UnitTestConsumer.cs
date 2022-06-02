using System;
using System.Threading.Tasks;
using FS.Extends;
using FS.MQ.RedisStream;
using FS.MQ.RedisStream.Attr;
using StackExchange.Redis;

namespace Farseer.Net.MQ.RedisStream.Test.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = true, Server = "test2", GroupName = "", QueueName = "test2", PullCount = 2, ConsumeThreadNums = 1)]
    public class UnitTestConsumer : IListenerMessage
    {
        public static long ID;
        public Task<bool> Consumer(StreamEntry[] messages, ConsumeContext context)
        {
            foreach (var message in messages)
            {
                ID = message.Values[0].ConvertType(0L);
                context.Ack(message: message);
            }

            return Task.FromResult(result: true);
        }

        public Task<bool> FailureHandling(StreamEntry[] messages, ConsumeContext context) => throw new NotImplementedException();
    }
}