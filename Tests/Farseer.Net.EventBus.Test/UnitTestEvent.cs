using System;
using System.Threading.Tasks;
using FS.Core.Abstract.EventBus;
using FS.EventBus.Attr;
using FS.Extends;

namespace Farseer.Net.EventBus.Test
{
    /// <summary>
    /// 测试事件
    /// </summary>
    [Consumer(EventName = "unit_test")]
    public class UnitTestEvent : IListenerMessage
    {
        public static long ID;
        public Task<bool> Consumer(object message, DomainEventArgs ea)
        {
            ID = message.ConvertType(0L);
            Console.WriteLine($"{ea.Id} 我订阅了test的消息：消息发送时间：{ea.CreateAt} 内容：{message}");
            return Task.FromResult(true);
        }
    }
}