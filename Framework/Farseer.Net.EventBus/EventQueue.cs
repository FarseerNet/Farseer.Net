using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;

namespace FS.EventBus
{
    /// <summary>
    /// 检查事件消息的队列是否有事件要处理
    /// </summary>
    public class EventQueue
    {
        /// <summary>
        /// 事件消息
        /// Key：EventName，Value：事件内容
        /// </summary>
        internal static readonly Dictionary<string, Queue<DomainEventArgs>> DicEventMessage = new();

        public void Check(string eventName)
        {
            if (!DicEventMessage.ContainsKey(eventName)) return;

            Task.Run(() =>
            {
                while (true)
                {
                    if (DicEventMessage[eventName].Count == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    var domainEventArgs = DicEventMessage[eventName].Dequeue();
                    EventHandle(eventName: eventName, domainEventArgs: domainEventArgs);
                }
            });
        }
        
        /// <summary>
        /// 事件处理
        /// </summary>
        internal static void EventHandle(string eventName, DomainEventArgs domainEventArgs)
        {
            // 找出当前事件的订阅者
            EventProduct.DicConsumer.TryGetValue(eventName, out var consumers);

            // 开始异步执行
            var dicHandleResult = consumers.ToDictionary(o => o, o => IocManager.GetService<IListenerMessage>(o).Consumer(domainEventArgs.Message.ToString(), domainEventArgs.Sender, domainEventArgs));
            Task.WaitAll(dicHandleResult.Select(o => (Task)o.Value).ToArray());

            // 将执行为true的移除
            while (dicHandleResult.Count > 0)
            {
                foreach (var consumer in consumers)
                {
                    // 不在字典中，说明上次执行成功时，被移除了
                    if (!dicHandleResult.ContainsKey(consumer)) continue;
                    // 执行成功，则从字典中移除
                    if (dicHandleResult[consumer].Result)
                    {
                        dicHandleResult.Remove(consumer);
                        continue;
                    }
                    // 执行失败，重新执行
                    dicHandleResult[consumer] = IocManager.GetService<IListenerMessage>(consumer).Consumer(domainEventArgs.Message.ToString(), domainEventArgs.Sender, domainEventArgs);
                }

                // 字典中存在执行失败的，休眠100ms。
                if (dicHandleResult.Count > 0) Thread.Sleep(100);
            }
        }
    }
}