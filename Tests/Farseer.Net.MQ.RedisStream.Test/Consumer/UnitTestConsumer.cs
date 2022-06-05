using System;
using System.Threading.Tasks;
using FS.Core.Abstract.MQ.RedisStream;
using FS.Extends;
using FS.MQ.RedisStream;
using FS.MQ.RedisStream.Attr;

namespace Farseer.Net.MQ.RedisStream.Test.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = true, Server = "UnitTest", GroupName = "", QueueName = "UnitTest", PullCount = 2, ConsumeThreadNums = 1)]
    public class UnitTestConsumer : IListenerMessage
    {
        public static long ID;
        public Task<bool> Consumer(ConsumeContext context)
        {
            foreach (var redisStreamMessage in context.RedisStreamMessages)
            {
                ID = redisStreamMessage.Message.ConvertType(0L);
                redisStreamMessage.Ack();
            }

            return Task.FromResult(result: true);
        }

        public Task<bool> FailureHandling(ConsumeContext context) => throw new NotImplementedException();
    }
}