using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.Abstract.MQ.Queue;
using FS.Extends;
using FS.MQ.Queue.Attr;

namespace Farseer.Net.MQ.Queue.Test.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = true, Name = "unit_test")]
    public class UnitTestConsumer : IListenerMessage
    {
        public static long ID;
        
        public Task<bool> Consumer(IEnumerable<object> queueList)
        {
            Console.WriteLine(value: $"消费到{queueList.Count()}条");
            ID = queueList.FirstOrDefault().ConvertType(0L);
            return Task.FromResult(true);
        }
        public Task<bool> FailureHandling(IEnumerable<object> messages) => throw new NotImplementedException();
    }
}