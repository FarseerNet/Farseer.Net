using System.Collections.Generic;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Core.Abstract.EventBus;

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
        private static readonly PooledDictionary<string, PooledList<EventQueue>> DicConsumer = new();

        public EventProduct(string eventName)
        {
            _eventName = eventName;
            DicConsumer.Add(_eventName, new PooledList<EventQueue>());
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        internal void Subscribe(string consumer)
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
                // 这里使用Task.Run，是因为如果使用Task.WaitAll，会导致SynchronizationContext上下文切换引导的死锁
                var consumerResult = Task.Run(async () => await consumer.EventHandle(domainEventArgs, false)).Result;
                result = result && consumerResult;
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