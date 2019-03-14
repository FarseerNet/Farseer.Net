using System;
using System.Threading;

namespace FS.Core.Queue.Core
{
    internal class DequeueSignal
    {
        /// <summary>开始出队消费事件通知</summary>
        private ManualResetEventSlim _deQueueEvent = new ManualResetEventSlim(false);//初始为无信号状态

        /// <summary>出队最大时间间隔,未满足出队阈值的情况下,达到此时间间隔也立即出队 </summary>
        private readonly TimeSpan _notifyDequeueTimeSpan;

        /// <summary>取消令牌 </summary>
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="notifyDequeueTimeSpan">出队时间间隔</param>
        /// <param name="cancellationToken">取消令牌</param>
        public DequeueSignal(TimeSpan notifyDequeueTimeSpan,CancellationToken cancellationToken)
        {
            this._notifyDequeueTimeSpan = notifyDequeueTimeSpan;
            this._cancellationToken = cancellationToken;
        }

        /// <summary>
        /// 发送出队信号
        /// </summary>
        public void SendSignal()
        {
            _deQueueEvent.Set();
        }

        /// <summary>
        /// 等待信号/取消令牌/超时
        /// 等到信号: 返回true
        /// 等待超时: 返回false
        /// 遇到取消: 抛出异常
        /// </summary>
        public bool WaitSignal()
        {
            return _deQueueEvent.Wait(_notifyDequeueTimeSpan, _cancellationToken);//等待直到 超时或者线程取消
        }

    }
}
