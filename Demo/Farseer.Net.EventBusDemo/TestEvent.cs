using System;
using System.Threading.Tasks;
using FS.Core.Abstract.EventBus;
using FS.EventBus.Attr;

namespace Farseer.Net.EventBusDemo
{
    /// <summary>
    /// 测试事件
    /// </summary>
    [Consumer(EventName = "test")]
    public class TestEvent : IListenerMessage
    {
        public async Task<bool> Consumer(object message, DomainEventArgs ea)
        {
            Console.WriteLine($"{ea.Id} 我订阅了test的消息：消息发送时间：{ea.CreateAt} 内容：{message}");
            return true;
        }
    }
}