using System;
using System.Collections.Generic;
using System.Threading;

namespace FS.Core.Queue.Core.AsyncQueue
{
    public abstract class AsyncQueue<T,E>
    {
        private AsyncQueueCore<T,E> QueueCore = null;

        protected AsyncQueue(int queueCapacity,TimeSpan notifyDequeueTimeSpan,int notifyDequeueSize = 10)
        {
            QueueCore = new AsyncQueueCore<T,E>(GetDequeueDataHandlers(), queueCapacity, notifyDequeueTimeSpan, notifyDequeueSize);
        }
        protected EnqueueResult Enqueue(T data,E extraData)
        {
            var asyncQueueData = new AsyncQueueData<T, E>();
            asyncQueueData.InitData(data,extraData);
            var result = QueueCore.Enqueue(asyncQueueData);
            if (result) return EnqueueResult.Sucess;
            return EnqueueResult.Overflow;
        }

        public void Start(CancellationToken cancellationToken)
        {
            QueueCore.StartDequeue(cancellationToken);
        }
            
        protected abstract Func<List<AsyncQueueData<T,E>>,int, bool> GetDequeueDataHandlers();
    }

}
