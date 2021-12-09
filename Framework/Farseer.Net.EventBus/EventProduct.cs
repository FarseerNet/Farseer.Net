using System.Collections.Generic;

namespace FS.EventBus
{
    public class EventProduct : IEventProduct
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        private readonly string _eventName;

        /// <summary>
        /// 事件订阅者
        /// Key：EventName，Value：订阅者Type.FullName
        /// </summary>
        internal static readonly Dictionary<string, List<string>> DicConsumer = new();

        public EventProduct(string eventName)
        {
            _eventName = eventName;
            EventQueue.DicEventMessage.Add(eventName, new Queue<DomainEventArgs>());
        }

        /// <summary>
        /// 发送事件消息（订阅方以同步的方式执行，执行完后，本方法返回结果）
        /// </summary>
        /// <param name="sender">事件发布者 </param>
        /// <param name="message"> 消息主体 </param>
        public bool Send(object sender, string message)
        {
            // 如果没有订阅者，则直接丢弃消息
            if (!DicConsumer.ContainsKey(_eventName)) return true;

            var domainEventArgs = new DomainEventArgs(sender, message);
            
            // 订阅者同步处理
            EventQueue.EventHandle(_eventName,domainEventArgs);
            
            return true;
        }

        /// <summary>
        /// 发送事件消息（订阅方以异步的方式执行，当前方法发送完后立即返回结果）
        /// </summary>
        /// <param name="sender">事件发布者 </param>
        /// <param name="message"> 消息主体 </param>
        /// <returns></returns>
        public bool SendAsync(object sender, string message)
        {
            // 如果没有订阅者，则直接丢弃消息
            if (!DicConsumer.ContainsKey(_eventName)) return true;

            // 将事件加入到本地队列中
            EventQueue.DicEventMessage[_eventName].Enqueue(new DomainEventArgs(sender, message));

            return true;
        }
    }
}