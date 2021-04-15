using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using FS.Core.Queue.Core;
using FS.Core.Queue.Core.SyncQueue;
using FS.DI;
using Microsoft.Extensions.Logging;

namespace FS.ElasticSearch.Queue
{
    /// <summary>
    /// 针对ES写盘的同步队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncQueueEs<T> : SyncQueue<T, QueueDataEs> where T : class
    {
        private ElasticClient             _elasticClient;
        public  Action<BulkResponse, int> EsSaveResultCallback = null;

        public SyncQueueEs(ElasticClient elasticClient, TimeSpan notifyDequeueTimeSpan, int notifyDequeueSize = 10) : base(notifyDequeueTimeSpan, notifyDequeueSize)
        {
            this._elasticClient = elasticClient;
        }

        /// <summary>
        /// 入队并等待处理结果
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indexName"></param>
        /// <param name="typeName"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public EnqueueResult EnqueueAndWaitHandleResult(T data, String indexName, String typeName, TimeSpan timeSpan)
        {
            QueueDataEs queueDataEs = new QueueDataEs();
            queueDataEs.IndexName = indexName;
            queueDataEs.TypeName  = typeName;

            return base.EnqueueAndWaitHandleResult(data, queueDataEs, timeSpan);
        }

        protected override Func<List<SyncQueueData<T, QueueDataEs>>, int, bool> GetDequeueDataHandlers()
        {
            return Save2Es;
        }

        private bool Save2Es(List<SyncQueueData<T, QueueDataEs>> lst, int queueCount)
        {
            try
            {
                bool bAllSucess = true;
                var indexTypeGroup = from data in lst
                    group data by new {data.ExtraData.IndexName, data.ExtraData.TypeName}; //按照天分组
                foreach (var indexType in indexTypeGroup)
                {
                    List<T> dataList = indexType.Select(qd => qd.Data).ToList<T>();
                    var     result   = _elasticClient.IndexMany(dataList, indexType.Key.IndexName);
                    EsSaveResultCallback?.Invoke(result, queueCount);

                    if (result.IsValid == false) bAllSucess = false;
                }

                return bAllSucess;
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<SyncQueueEs<T>>().LogError(e, e.ToString());
                return false;
            }
        }
    }
}