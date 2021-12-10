using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private static readonly Dictionary<string, ConcurrentQueue<DomainEventArgs>> DicEventMessage = new();
        /// <summary>
        /// 失败的队列
        /// Key：Type.FullName，Value：事件内容
        /// </summary>
        private static readonly Dictionary<string, ConcurrentQueue<DomainEventArgs>> DicEventFailMessage = new();
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
        public void Push(DomainEventArgs args)
        {
            DicEventMessage[Consumer].Enqueue(args);
        }

        public void Work()
        {
            // 执行当前队列
            Task.Run(async () =>
            {
                while (true)
                {
                    if (DicEventMessage[Consumer].Count == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    DicEventMessage[Consumer].TryDequeue(out var domainEventArgs);
                    EventHandle(domainEventArgs: domainEventArgs, true);
                }
            });
            
            // 执行失败队列
            Task.Run(async () =>
            {
                while (true)
                {
                    if (DicEventFailMessage[Consumer].Count == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    DicEventFailMessage[Consumer].TryDequeue(out var domainEventArgs);
                    EventHandle(domainEventArgs: domainEventArgs, true);
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
                result = await IocManager.GetService<IListenerMessage>(Consumer).Consumer(domainEventArgs.Message.ToString(), domainEventArgs.Sender, domainEventArgs);
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<EventQueue>().LogError(e, $"订阅端：{Consumer}，执行异常，消息：{JsonConvert.SerializeObject(domainEventArgs)}，", e.Message);
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