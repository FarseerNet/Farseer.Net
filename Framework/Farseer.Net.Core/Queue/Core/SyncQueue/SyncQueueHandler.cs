using System;
using System.Threading;

namespace FS.Core.Queue.Core.SyncQueue
{
    /// <summary>
    /// 同步处理器,每个线程一个处理器,负责本线程中的数据入队以及出队处理结果.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    internal class SyncQueueHandler<T,E>
    {
        private readonly SyncQueueData<T, E> _queueData;
        private Thread _thread = null;
        private readonly SyncQueueCore<T,E> _queueCore;

       /* public QueueData<T, E> GetCacheData() 
        {
            return _queueData;
        }*/

        public SyncQueueHandler(Thread thread, SyncQueueCore<T,E> queueCore)
        {
            this._queueCore = queueCore;
            this._thread = thread;
            this._queueData = new SyncQueueData<T, E>();
        }
        
        public EnqueueResult EnqueueAndWaitHandleResult(T data,E extraData,TimeSpan timeSpan)
        {
            if (_queueData.FinishedHandle() == false) return EnqueueResult.Busy;//当前线程的数据尚未处理完毕.

            _queueData.InitData(data,extraData);
            _queueData.BeforeEnqueue();
            _queueCore.Enqueue(_queueData);
            bool result = _queueData.WaitHandled(timeSpan);
            if (result == true) return EnqueueResult.Sucess;
            else return EnqueueResult.Timeout;

        }
    }
}
