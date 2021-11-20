using System;
using System.Collections.Generic;
using System.Linq;
using FS.Core.Async;
using FS.Core.LinkTrack;
using FS.LinkTrack.Dal;

namespace FS.LinkTrack
{
    public class LinkTrackQueue : BaseAsyncQueue<LinkTrackContext>, ILinkTrackQueue
    {
        internal LinkTrackQueue() : base(maxQueueSize: 500000, callBackListCapacity: 100, sleepMs: 2000)
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
        protected override void OnDequeue(List<LinkTrackContext> lst, int remainCount)
        {
            // 设置C#的调用链
            foreach (var linkTrackContext in lst) linkTrackContext.List.ForEach(action: o => o.SetCallStackTrace());

            LinkTrackEsContext.Data.LinkTrackContext.Insert(lst: lst.Select(selector: o => new LinkTrackContextPO
                                                                    {
                                                                        Id           = $"{o.AppId}_{o.ContextId}",
                                                                        AppId        = o.AppId,
                                                                        ParentAppId  = o.ParentAppId,
                                                                        ContextId    = o.ContextId,
                                                                        List         = o.List,
                                                                        StartTs      = o.StartTs,
                                                                        EndTs        = o.EndTs,
                                                                        Domain       = o.Domain,
                                                                        Path         = o.Path,
                                                                        Method       = o.Method,
                                                                        Headers      = o.Headers,
                                                                        ContentType  = o.ContentType,
                                                                        RequestBody  = o.RequestBody,
                                                                        ResponseBody = o.ResponseBody,
                                                                        RequestIp    = o.RequestIp
                                                                    })
                                                                    .ToList());

            // 依赖外部系统的，单独存储，用于统计慢查询
            AddSlowQuery(lst: lst);
        }

        /// <summary>
        /// 依赖外部系统的，单独存储，用于统计慢查询
        /// </summary>
        private void AddSlowQuery(List<LinkTrackContext> lst)
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
                            if (string.IsNullOrWhiteSpace(linkTrackDetail.DbLinkTrackDetail.Sql)) break;
                            lstSlowQuery.Add(new SlowQueryPO
                            {
                                AppId       = linkTrackContext.AppId,
                                ContextId   = linkTrackContext.ContextId,
                                CallType    = linkTrackDetail.CallType,
                                UseTs       = linkTrackDetail.UseTs,
                                StartTs     = linkTrackDetail.StartTs,
                                DbName      = linkTrackDetail.DbLinkTrackDetail.DataBaseName,
                                DbTableName = linkTrackDetail.DbLinkTrackDetail.TableName,
                                DbSql       = linkTrackDetail.DbLinkTrackDetail.Sql,
                                DbSqlParam  = linkTrackDetail.DbLinkTrackDetail.SqlParam,
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
            LinkTrackEsContext.Data.SlowQuery.Insert(lstSlowQuery);
        }
    }
}