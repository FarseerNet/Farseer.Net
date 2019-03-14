using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FS.Core.Async
{
    /// <summary>
    /// 进程内异步队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseAsyncQueue<T>
    {
        /// <summary>并发队列</summary>
        private readonly ConcurrentQueue<T> _concurrentQueue = new ConcurrentQueue<T>();
        /// <summary>出队列任务</summary>
        private Task _dequeueTask;
        /// <summary>队列最大长度</summary>
        private readonly int _maxQueueSize;
        /// <summary>出队的数据</summary>
        private readonly List<T> _callBackList;
        /// <summary> 下一次回调的时间间隔(毫秒) </summary>
        protected int NextCallBackIntervalMs;
        /// <summary> 队列数据元素个数 </summary>
        public int QueueCount => _concurrentQueue.Count;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="maxQueueSize">队列最大长度,溢出则丢失</param>
        /// <param name="callBackListCapacity">每次回调出队最大数量</param>
        protected BaseAsyncQueue(int maxQueueSize = 5000, int callBackListCapacity = 10)
        {
            _maxQueueSize = maxQueueSize;
            _callBackList = new List<T>(callBackListCapacity);
        }

        /// <summary>入队,满了就丢掉新数据</summary>
        public bool Enqueue(T tobj)
        {
            if (QueueCount >= _maxQueueSize) return false;
            _concurrentQueue.Enqueue(tobj);
            return true;
        }

        /// <summary>开始异步出队</summary>
        public void StartDequeue(CancellationToken cancellationTokentoken)
        {
            _dequeueTask = new Task(() => LoopDequeue(cancellationTokentoken), TaskCreationOptions.LongRunning);
            _dequeueTask.Start();
        }

        /// <summary>
        ///     回调 数据给用户处理
        /// </summary>
        /// <param name="callbackList">回调的 数据列表</param>
        /// <param name="remainCount">队列中当前剩余多少要处理</param>
        protected abstract void OnDequeue(List<T> callbackList, int remainCount);

        /// <summary>
        ///     循环从队列中拉出数据,回调给用户处理
        /// </summary>
        /// <param name="token">取消令牌</param>
        private void LoopDequeue(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested(); //取消退出线程函数
                try
                {
                    DeQueue(_callBackList); //队列数据出队保存到回调数据列表
                    NextCallBackIntervalMs = 0;
                    OnDequeue(_callBackList, QueueCount); //交由用户处理
                    SafeSleep(token);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        ///     从队列中拉出指定数量的数据,放到回调数据队列中,直到队列为空,或者回调队列满
        /// </summary>
        /// <param name="callbackList">回调数据队列</param>
        /// <returns></returns>
        private void DeQueue(List<T> callbackList)
        {
            var maxSize = callbackList.Capacity;
            if (callbackList == null) throw new System.Exception("回调数据队列为Null");
            callbackList.Clear();
            for (var i = 0; i < maxSize; i++)
                if (_concurrentQueue.TryDequeue(out T tempObj)) { callbackList.Add(tempObj); }
                else
                {
                    break;
                }
        }

        /// <summary>
        ///     线程休眠,5ms检测一次取消令牌
        /// </summary>
        private void SafeSleep(CancellationToken ct)
        {
            try
            {
                long leftSleep = NextCallBackIntervalMs;
                var start = Environment.TickCount;
                while (true)
                {
                    ct.ThrowIfCancellationRequested(); //检测取消令牌,取消执行

                    if (leftSleep < 5)
                    {
                        if (leftSleep >= 0) Thread.Sleep((int)leftSleep);
                        else Thread.Sleep(1);
                        return;
                    }
                    Thread.Sleep(5);
                    var stop = Environment.TickCount;
                    leftSleep = NextCallBackIntervalMs - (stop - start);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
