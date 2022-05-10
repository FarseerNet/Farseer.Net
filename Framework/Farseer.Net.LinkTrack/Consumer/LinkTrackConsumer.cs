using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.LinkTrack.Dal;
using FS.MQ.Queue;
using FS.MQ.Queue.Attr;
using Mapster;

namespace FS.LinkTrack.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = true, Name = "LinkTrackQueue", PullCount = 1000, SleepTime = 500)]
    public class LinkTrackConsumer : IListenerMessage
    {
        public async Task<bool> Consumer(List<object> queueList)
        {
            var lst = queueList.Select(o => (LinkTrackContext)o).ToList();
            if (lst.Count == 0) return true;

            // 设置C#的调用链
            foreach (var linkTrackContext in lst) linkTrackContext.List.ForEach(action: o => o.SetCallStackTrace());

            await LinkTrackEsContext.Data.LinkTrackContext.InsertAsync(lst.Select(o => o.Adapt<LinkTrackContextPO>()).ToList());
            // 依赖外部系统的，单独存储，用于统计慢查询
            await AddSlowQuery(lst: lst);

            return true;
        }

        public Task<bool> FailureHandling(List<object> messages) => Task.FromResult(false);

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
                                AppName         = linkTrackContext.AppName,
                                AppId           = linkTrackContext.AppId,
                                AppIp           = linkTrackContext.AppIp,
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
                                AppName   = linkTrackContext.AppName,
                                AppId     = linkTrackContext.AppId,
                                AppIp     = linkTrackContext.AppIp,
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
                                AppName     = linkTrackContext.AppName,
                                AppId       = linkTrackContext.AppId,
                                AppIp       = linkTrackContext.AppIp,
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
                                AppName         = linkTrackContext.AppName,
                                AppId           = linkTrackContext.AppId,
                                AppIp           = linkTrackContext.AppIp,
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
                                AppName   = linkTrackContext.AppName,
                                AppId     = linkTrackContext.AppId,
                                AppIp     = linkTrackContext.AppIp,
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
                                AppName   = linkTrackContext.AppName,
                                AppId     = linkTrackContext.AppId,
                                AppIp     = linkTrackContext.AppIp,
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
            if (lstSlowQuery.Count == 0) return Task.FromResult(true);
            return LinkTrackEsContext.Data.SlowQuery.InsertAsync(lstSlowQuery);
        }
    }
}