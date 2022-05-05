using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using Microsoft.Extensions.Logging;

namespace FS.Core.Async
{
    /// <summary>
    ///     进程内异步队列
    /// </summary>
    public abstract class BaseAsyncQueue<T>
    {
        /// <summary> 出队的数据 </summary>
        private readonly List<T> _callBackList;

        /// <summary> 并发队列 </summary>
        private readonly ConcurrentQueue<T> _concurrentQueue = new();

        /// <summary> 队列最大长度 </summary>
        private readonly int _maxQueueSize;
        private readonly int _callBackListCapacity;

        private readonly int _sleepMs;

        /// <summary> 出队列任务 </summary>
        private Task _dequeueTask;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="maxQueueSize"> 队列最大长度,溢出则丢失 </param>
        /// <param name="callBackListCapacity"> 每次回调出队最大数量 </param>
        protected BaseAsyncQueue(int maxQueueSize = 5000, int callBackListCapacity = 10, int sleepMs = 500)
        {
            _maxQueueSize         = maxQueueSize;
            _callBackListCapacity = callBackListCapacity;
            _callBackList         = new List<T>(capacity: callBackListCapacity);
            _sleepMs              = sleepMs;
        }

        /// <summary> 队列数据元素个数 </summary>
        public int QueueCount => _concurrentQueue.Count;

        /// <summary> 入队,满了就丢掉新数据 </summary>
        public bool Enqueue(T obj)
        {
            if (obj        == null) return false;
            if (QueueCount >= _maxQueueSize) return false;
            _concurrentQueue.Enqueue(item: obj);
            return true;
        }

        /// <summary> 开始异步出队 </summary>
        public void StartDequeue(CancellationToken cancellationToken) => LoopDequeue(cancellationToken);

        /// <summary>
        ///     回调 数据给用户处理
        /// </summary>
        /// <param name="callbackList"> 回调的 数据列表 </param>
        /// <param name="remainCount"> 队列中当前剩余多少要处理 </param>
        protected abstract Task OnDequeue(List<T> callbackList, int remainCount);

        /// <summary>
        ///     循环从队列中拉出数据,回调给用户处理
        /// </summary>
        /// <param name="token"> 取消令牌 </param>
        private void LoopDequeue(CancellationToken token)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested(); //取消退出线程函数
                    try
                    {
                        DeQueue(); //队列数据出队保存到回调数据列表

                        if (_callBackList.Count > 0)
                        {
                            await OnDequeue(callbackList: _callBackList, remainCount: QueueCount); //交由用户处理
                            _callBackList.Clear();
                        }
                    }
                    catch (System.Exception e)
                    {
                        IocManager.Instance.Logger<BaseAsyncQueue<T>>().LogError(e.Message, e);
                    }

                    Thread.Sleep(millisecondsTimeout: _sleepMs);
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        ///     从队列中拉出指定数量的数据,放到回调数据队列中,直到队列为空,或者回调队列满
        /// </summary>
        private void DeQueue()
        {
            if (_callBackList == null) throw new System.Exception(message: "回调数据队列为Null");
            _callBackList.Clear();

            while (_callBackList.Count < _callBackListCapacity && _concurrentQueue.TryDequeue(result: out var tempObj))
            {
                _callBackList.Add(item: tempObj);
            }
        }
    }
}