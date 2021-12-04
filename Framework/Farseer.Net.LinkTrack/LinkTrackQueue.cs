using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.Async;
using FS.Core.LinkTrack;
using FS.Extends;
using FS.LinkTrack.Dal;

namespace FS.LinkTrack
{
    public class LinkTrackQueue : BaseAsyncQueue<LinkTrackContext>, ILinkTrackQueue
    {
        internal LinkTrackQueue() : base(maxQueueSize: 500000, callBackListCapacity: 1000, sleepMs: 500)
        {
        }

        /// <summary>
        ///     将链路追踪写入队列
        /// </summary>
        public void Enqueue() => base.Enqueue(obj: FsLinkTrack.Current.Get());

        /// <summary>
        ///     回调 数据给用户处理
        /// </summary>
        /// <param name="lst"> 回调的 数据列表 </param>
        /// <param name="remainCount"> 队列中当前剩余多少要处理 </param>
        protected override Task OnDequeue(List<LinkTrackContext> lst, int remainCount)
        {
            // 设置C#的调用链
            foreach (var linkTrackContext in lst) linkTrackContext.List.ForEach(action: o => o.SetCallStackTrace());

            return Task.WhenAll(
                                LinkTrackEsContext.Data.LinkTrackContext.InsertAsync(lst.Select(o => o.Map<LinkTrackContextPO>()).ToList()),
                                // 依赖外部系统的，单独存储，用于统计慢查询
                                AddSlowQuery(lst: lst));
        }

        /// <summary>
        /// 依赖外部系统的，单独存储，用于统计慢查询
        /// </summary>
        private Task AddSlowQuery(List<LinkTrackContext> lst)
        {
            var lstSlowQuery = new List<SlowQueryPO>();
            foreach (var linkTrackContext in lst)
            {
                foreach (var linkTrackDetail in linkTrackContext.List)
                {
                    switch (linkTrackDetail.CallType)
                    {
                        case EumCallType.HttpClient:
                            lstSlowQuery.Add(new SlowQueryPO
                            {
                                AppId           = linkTrackContext.AppId,
                                ContextId       = linkTrackContext.ContextId,
                                CallType        = linkTrackDetail.CallType,
                                UseTs           = linkTrackDetail.UseTs,
                                StartTs         = linkTrackDetail.StartTs,
                                HttpUrl         = linkTrackDetail.Data["Url"],
                                HttpMethod      = linkTrackDetail.Data["Method"],
                                HttpRequestBody = linkTrackDetail.Data["RequestBody"]
                            });
                            break;
                        case EumCallType.GrpcClient:
                            lstSlowQuery.Add(new SlowQueryPO
                            {
                                AppId     = linkTrackContext.AppId,
                                ContextId = linkTrackContext.ContextId,
                                CallType  = linkTrackDetail.CallType,
                                UseTs     = linkTrackDetail.UseTs,
                                StartTs   = linkTrackDetail.StartTs,
                                GrpcUrl   = linkTrackDetail.Data["Server"] + "/" + linkTrackDetail.Data["Action"],
                            });
                            break;
                        case EumCallType.Database:
                            // Sql为空，则不记录
                            if (!linkTrackDetail.Data.ContainsKey("Sql")) break;
                            lstSlowQuery.Add(new SlowQueryPO
                            {
                                AppId       = linkTrackContext.AppId,
                                ContextId   = linkTrackContext.ContextId,
                                CallType    = linkTrackDetail.CallType,
                                UseTs       = linkTrackDetail.UseTs,
                                StartTs     = linkTrackDetail.StartTs,
                                DbName      = linkTrackDetail.Data["DataBaseName"],
                                DbTableName = linkTrackDetail.Data["TableName"],
                                DbSql       = linkTrackDetail.Data["Sql"],
                            });
                            break;
                        case EumCallType.Redis:
                            lstSlowQuery.Add(new SlowQueryPO
                            {
                                AppId           = linkTrackContext.AppId,
                                ContextId       = linkTrackContext.ContextId,
                                CallType        = linkTrackDetail.CallType,
                                UseTs           = linkTrackDetail.UseTs,
                                StartTs         = linkTrackDetail.StartTs,
                                RedisKey        = linkTrackDetail.Data["RedisKey"],
                                RedisHashFields = linkTrackDetail.Data["RedisHashFields"],
                            });
                            break;
                        case EumCallType.Mq:
                            lstSlowQuery.Add(new SlowQueryPO
                            {
                                AppId     = linkTrackContext.AppId,
                                ContextId = linkTrackContext.ContextId,
                                CallType  = linkTrackDetail.CallType,
                                UseTs     = linkTrackDetail.UseTs,
                                StartTs   = linkTrackDetail.StartTs,
                                MqTopic   = linkTrackDetail.CallMethod,
                            });
                            break;
                        case EumCallType.Elasticsearch:
                            lstSlowQuery.Add(new SlowQueryPO
                            {
                                AppId     = linkTrackContext.AppId,
                                ContextId = linkTrackContext.ContextId,
                                CallType  = linkTrackDetail.CallType,
                                UseTs     = linkTrackDetail.UseTs,
                                StartTs   = linkTrackDetail.StartTs,
                                EsMethod  = linkTrackDetail.CallMethod,
                            });
                            break;
                    }
                }
            }
            return LinkTrackEsContext.Data.SlowQuery.InsertAsync(lstSlowQuery);
        }
    }
}