using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Core.Abstract.EventBus;
using FS.DI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FS.EventBus
{
    /// <summary>
    /// 检查事件消息的队列是否有事件要处理
    /// </summary>
    public class EventQueue
    {
        /// <summary>
        /// 事件消息
        /// Key：Type.FullName，Value：事件内容
        /// </summary>
        private static readonly PooledDictionary<string, ConcurrentQueue<DomainEventArgs>> DicEventMessage = new();
        /// <summary>
        /// 失败的队列
        /// Key：Type.FullName，Value：事件内容
        /// </summary>
        private static readonly PooledDictionary<string, ConcurrentQueue<DomainEventArgs>> DicEventFailMessage = new();
        /// <summary>
        /// 订阅端
        /// </summary>
        private string Consumer { get; }

        public EventQueue(string consumer)
        {
            this.Consumer = consumer;
            DicEventMessage.Add(consumer, new ConcurrentQueue<DomainEventArgs>());
            DicEventFailMessage.Add(consumer, new ConcurrentQueue<DomainEventArgs>());
        }

        /// <summary>
        /// 将事件加入到订阅端的队列中
        /// </summary>
        public void Push(DomainEventArgs args) => DicEventMessage[Consumer].Enqueue(args);

        /// <summary>
        /// 订阅者遍历事件，并消费事件
        /// </summary>
        public void Work()
        {
            // 执行当前队列
            Task.Run(() =>
            {
                while (true)
                {
                    if (DicEventMessage[Consumer].Count == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    DicEventMessage[Consumer].TryDequeue(out var domainEventArgs);
                    // 使用异步执行，不需要等待：同一个订阅者，不同事件，允许多线程执行
                    _ = EventHandle(domainEventArgs: domainEventArgs, true);
                }
            });

            // 执行失败队列
            Task.Run(() =>
            {
                while (true)
                {
                    if (DicEventFailMessage[Consumer].Count == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    DicEventFailMessage[Consumer].TryDequeue(out var domainEventArgs);
                    // 使用异步执行，不需要等待：同一个订阅者，不同事件，允许多线程执行
                    _ = EventHandle(domainEventArgs: domainEventArgs, true);
                }
            });
        }

        /// <summary>
        /// 事件处理，处理失败后，会加入到失败的错误队列中
        /// </summary>
        /// <param name="domainEventArgs">事件体</param>
        /// <param name="useAsyncQueue">使用异步队列 </param>
        internal async Task<bool> EventHandle(DomainEventArgs domainEventArgs, bool useAsyncQueue)
        {
            var result = false;
            try
            {
                if (useAsyncQueue && domainEventArgs.ErrorCount > 0)
                {
                    await Task.Delay((int)Math.Pow(10, domainEventArgs.ErrorCount));
                }
                var consumerService = IocManager.GetService<IListenerMessage>(Consumer);
                result = await consumerService.Consumer(domainEventArgs.Message, domainEventArgs);
                IocManager.Instance.Release(consumerService);
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<EventQueue>().LogError(e, $"订阅端：{Consumer}，执行异常，消息：{e.Message}，");
            }
            if (!result)
            {
                domainEventArgs.ErrorCount++;
                if (useAsyncQueue) DicEventFailMessage[Consumer].Enqueue(domainEventArgs);
            }
            return result;
        }
    }
}