using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FS.Core.Queue.Core.SyncQueue
{
    public abstract class SyncQueue<T,E>
    {
        private SyncQueueCore<T,E> QueueCore = null;

        protected SyncQueue(TimeSpan notifyDequeueTimeSpan,int notifyDequeueSize = 10)
        {
            QueueCore = new SyncQueueCore<T,E>(GetDequeueDataHandlers(), notifyDequeueTimeSpan, notifyDequeueSize);
        }

        private  ConcurrentDictionary<int, SyncQueueHandler<T,E>> QueueHandlers = new ConcurrentDictionary<int, SyncQueueHandler<T,E>>();

        private SyncQueueHandler<T, E> InitCurrentHandler()
        {
            return new SyncQueueHandler<T, E>(Thread.CurrentThread, QueueCore);
        }

        private SyncQueueHandler<T, E>  GetCurrentHandler()
        {
            SyncQueueHandler<T, E> threadCache = QueueHandlers.GetOrAdd(Thread.CurrentThread.ManagedThreadId,
                InitCurrentHandler());
            return threadCache;
        }
        protected EnqueueResult EnqueueAndWaitHandleResult(T data,E extraData,TimeSpan timeSpan)
        {
            var handler = GetCurrentHandler();
            return handler.EnqueueAndWaitHandleResult(data,extraData,timeSpan);
        }

        public void Start(CancellationToken cancellationToken)
        {
            QueueCore.StartDequeue(cancellationToken);
        }
            
        protected abstract Func<List<SyncQueueData<T,E>>,int, bool> GetDequeueDataHandlers();
    }

}
