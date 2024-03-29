using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.Abstract.MQ.Queue;
using FS.MQ.Queue;
using FS.MQ.Queue.Attr;

namespace Farseer.Net.MQ.QueueDemo.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = true, Name = "test")]
    public class TestConsumer : IListenerMessage
    {
        public Task<bool> Consumer(IEnumerable<object> queueList)
        {
            Console.WriteLine(value: $"消费到{queueList.Count()}条");
            return Task.FromResult(result: true);
        }
        public Task<bool> FailureHandling(IEnumerable<object> messages) => throw new NotImplementedException();
    }
}