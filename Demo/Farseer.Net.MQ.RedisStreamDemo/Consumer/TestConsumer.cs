using System;
using System.Threading.Tasks;
using FS.MQ.RedisStream;
using FS.MQ.RedisStream.Attr;

namespace Farseer.Net.MQ.RedisStreamDemo.Consumer
{
    /// <summary>
    /// 消费客户端
    /// </summary>
    [Consumer(Enable = true, RedisName = "default", GroupName = "test", QueueName = "test", PullCount = 2, ConsumeThreadNums = 1)]
    public class TestConsumer : IListenerMessage
    {
        public Task<bool> Consumer(string[] messages, ConsumeContext ea)
        {
            foreach (var message in messages)
            {
                System.Console.WriteLine("接收到信息为:" + message);
            }

            return Task.FromResult(true);
        }

        public Task<bool> FailureHandling(string[] messages, ConsumeContext ea) => throw new NotImplementedException();
    }
}