using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using FS.Core.Queue.Core;
using FS.Core.Queue.Core.AsyncQueue;
using FS.DI;

namespace FS.ElasticSearch.Queue
{
    /// <summary>
    /// 针对ES写盘的异步队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncQueueEs<T> : AsyncQueue<T, QueueDataEs> where T:class 
    {
        private readonly ElasticClient _elasticClient;
        public Action<BulkResponse,int> EsSaveResultCallback = null;
        public AsyncQueueEs(ElasticClient elasticClient,int queueCapacity, TimeSpan notifyDequeueTimeSpan, int notifyDequeueSize = 10) : base(queueCapacity,notifyDequeueTimeSpan, notifyDequeueSize)
        {
            this._elasticClient = elasticClient;
        }

        public EnqueueResult Enqueue(T data,String indexName,String typeName, TimeSpan timeSpan)
        {
            QueueDataEs queueDataEs = new QueueDataEs();
            queueDataEs.IndexName = indexName;
            queueDataEs.TypeName = typeName;

            return base.Enqueue(data,queueDataEs);
        }
        protected override Func<List<AsyncQueueData<T, QueueDataEs>>,int, bool> GetDequeueDataHandlers()
        {
            return Save2Es;
        }

        private bool Save2Es(List<AsyncQueueData<T, QueueDataEs>> lst,int queueCount)
        {
            try
            {
                //int nCount = 0;
                bool bAllSucess = true;
                var indexTypeGroup = from data in lst
                    group data by new { data.ExtraData.IndexName, data.ExtraData.TypeName }; //按照天分组
                foreach (var indexType in indexTypeGroup)
                {
                    List<T> dataList = indexType.Select(qd => qd.Data).ToList<T>();
                    
                    var result = _elasticClient.IndexMany(dataList, indexType.Key.IndexName);
                    
                    EsSaveResultCallback?.Invoke(result, queueCount);
                    
                    if (result.IsValid == false) bAllSucess = false;
                }
                return bAllSucess;
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger.Error(e.ToString(), e);
                return false;
            }
        }

    }
}
