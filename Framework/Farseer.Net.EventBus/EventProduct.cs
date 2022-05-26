using System.Collections.Generic;
using System.Threading.Tasks;
using FS.Core.EventBus;

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
        private static readonly Dictionary<string, List<EventQueue>> DicConsumer = new();

        public EventProduct(string eventName)
        {
            _eventName = eventName;
            DicConsumer.Add(_eventName, new List<EventQueue>());
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(string consumer)
        {
            var eventQueue = new EventQueue(consumer);
            DicConsumer[_eventName].Add(eventQueue);
            eventQueue.Work();
        }

        /// <summary>
        /// 发送事件消息（订阅方以同步的方式执行，执行完后，本方法返回结果）
        /// </summary>
        /// <param name="sender">事件发布者 </param>
        /// <param name="message"> 消息主体 </param>
        /// <returns>返回false，订阅方处理失败</returns>
        public bool SendSync(object sender, object message)
        {
            // 生成事件体
            var domainEventArgs = new DomainEventArgs(sender, message);

            var result = true;
            // 找出事件的订阅方
            foreach (var consumer in DicConsumer[_eventName])
            {
                // 订阅者同步处理
                var eventHandle = consumer.EventHandle(domainEventArgs, false);
                Task.WaitAll(eventHandle);
                result = result && eventHandle.Result;
            }
            return result;
        }

        /// <summary>
        /// 发送事件消息（订阅方以异步的方式执行，订阅方执行失败时，会加入到失败队列，继续重试）
        /// </summary>
        /// <param name="sender">事件发布者 </param>
        /// <param name="message"> 消息主体 </param>
        public bool SendAsync(object sender, object message)
        {
            // 将事件加入到所有订阅方的队列中
            foreach (var consumer in DicConsumer[_eventName])
            {
                consumer.Push(new DomainEventArgs(sender, message));
            }
            return true;
        }
    }
}