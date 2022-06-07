using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Collections.Pooled;
using FS.Extends;
using Newtonsoft.Json;

namespace FS.Core.LinkTrack
{
    /// <summary>
    ///     多线程共享上下文
    /// </summary>
    /// <remarks> 测 </remarks>
    public class FsLinkTrack
    {
        private static readonly AsyncLocal<LinkTrackContext> AsyncLocal = new();

        /// <summary>
        ///     静态属性
        /// </summary>
        private static FsLinkTrack _current;

        /// <summary>
        ///     静态属性
        /// </summary>
        public static FsLinkTrack Current => _current ??= new FsLinkTrack();

        /// <summary>
        ///     获取数据
        /// </summary>
        public LinkTrackContext Get() => AsyncLocal.Value;

        /// <summary>
        ///     写入数据
        /// </summary>
        public void Set(LinkTrackDetail linkTrackDetail)
        {
            if (AsyncLocal.Value == null) return;
            linkTrackDetail._stackTrace = new StackTrace(true);
            AsyncLocal.Value.List.Add(item: linkTrackDetail);
        }

        /// <summary>
        ///     追踪Mq消费
        /// </summary>
        public static TrackEnd TrackMqConsumer(string endPort, string queueName, string method, string message)
        {
            AsyncLocal.Value = new LinkTrackContext
            {
                LinkType      = EumLinkType.Consumer,
                ParentAppName = "",
                ContextId     = SnowflakeId.GenerateId.ToString(),
                StartTs       = DateTime.Now.ToTimestamps(),
                List          = new PooledList<LinkTrackDetail>(),
                Method        = method,
                Path          = queueName,
                Domain        = endPort,
                RequestIp     = FarseerApplication.AppIp.FirstOrDefault(),
                RequestBody   = message,
                ContentType   = "",
                StatusCode    = "",
            };
            return new TrackEnd(linkTrackContext: AsyncLocal.Value);
        }

        /// <summary>
        ///     追踪Fss
        /// </summary>
        internal static TrackEnd TrackFss(string clientHost, string jobName, int taskGroupId, IDictionary<string, string> taskData)
        {
            AsyncLocal.Value = new LinkTrackContext
            {
                LinkType      = EumLinkType.Fss,
                ParentAppName = "",
                ContextId     = SnowflakeId.GenerateId.ToString(),
                StartTs       = DateTime.Now.ToTimestamps(),
                List          = new PooledList<LinkTrackDetail>(),
                Method        = jobName,
                Path          = $"{taskGroupId}",
                Domain        = clientHost,
                RequestIp     = FarseerApplication.AppIp.FirstOrDefault(),
                RequestBody   = JsonConvert.SerializeObject(taskData ?? new Dictionary<string, string>()),
                ContentType   = ""
            };
            return new TrackEnd(linkTrackContext: AsyncLocal.Value);
        }

        /// <summary>
        ///     追踪BackgroundService
        /// </summary>
        internal static TrackEnd TrackBackgroundService(string jobName)
        {
            AsyncLocal.Value = new LinkTrackContext
            {
                LinkType      = EumLinkType.BackgroundService,
                ParentAppName = "",
                ContextId     = SnowflakeId.GenerateId.ToString(),
                StartTs       = DateTime.Now.ToTimestamps(),
                List          = new PooledList<LinkTrackDetail>(),
                RequestIp     = FarseerApplication.AppIp.FirstOrDefault(),
                Method        = "",
                Path          = jobName,
                ContentType   = ""
            };
            return new TrackEnd(linkTrackContext: AsyncLocal.Value);
        }

        /// <summary>
        ///     追踪Job
        /// </summary>
        internal static TrackEnd TrackJob(string jobName)
        {
            AsyncLocal.Value = new LinkTrackContext
            {
                LinkType      = EumLinkType.Job,
                ParentAppName = "",
                ContextId     = SnowflakeId.GenerateId.ToString(),
                StartTs       = DateTime.Now.ToTimestamps(),
                List          = new PooledList<LinkTrackDetail>(),
                RequestIp     = FarseerApplication.AppIp.FirstOrDefault(),
                Method        = "",
                Path          = jobName,
                ContentType   = ""
            };
            return new TrackEnd(linkTrackContext: AsyncLocal.Value);
        }

        /// <summary>
        ///     追踪ApiServer
        /// </summary>
        public static TrackEnd TrackApiServer(string contextId, string parentAppId, string domain, string path, string method, string contentType, IDictionary<string, string> headerDictionary, string requestBody, string requestIp)
        {
            if (string.IsNullOrWhiteSpace(contextId)) contextId = SnowflakeId.GenerateId.ToString();
            // 移除charset的类型
            if (contentType.Contains(value: "charset"))
            {
                var contentTypes = contentType.Split(';').ToList();
                contentTypes.RemoveAll(match: o => o.Contains(value: "charset"));

                // 如果有application，则直接获取
                var application = contentTypes.Find(match: o => o.Contains(value: "application"));
                contentType = !string.IsNullOrWhiteSpace(value: application) ? application : string.Join(separator: ";", values: contentTypes);
            }

            AsyncLocal.Value = new LinkTrackContext()
            {
                LinkType      = EumLinkType.ApiServer,
                ParentAppName = parentAppId ?? "",
                ContextId     = contextId,
                StartTs       = DateTime.Now.ToTimestamps(),
                List          = new PooledList<LinkTrackDetail>(),
                Domain        = domain,
                Path          = path,
                Method        = method,
                ContentType   = contentType,
                Headers       = headerDictionary as PooledDictionary<string, string> ?? headerDictionary.ToPooledDictionary(),
                RequestBody   = requestBody,
                RequestIp     = requestIp,
                StatusCode    = ""
            };

            return new TrackEnd(linkTrackContext: AsyncLocal.Value);
        }

        /// <summary>
        ///     追踪数据库
        /// </summary>
        public static TrackEnd TrackDatabase(string method, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType   = EumCallType.Database,
                CallMethod = method,
                Data =
                {
                    ["DataBaseName"] = dbName,
                    ["TableName"]    = tableName,
                    ["CommandType"]  = cmdType.ToString(),
                    ["Sql"]          = sql
                }
            };
            linkTrackDetail.SetDbParam(param);

            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        ///     追踪数据库
        /// </summary>
        internal static TrackEnd TrackDatabase(string method, string connectionString)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType   = EumCallType.Database,
                CallMethod = method,
                Data =
                {
                    ["ConnectionString"] = connectionString
                }
            };

            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        ///     追踪Redis
        /// </summary>
        public static TrackEnd TrackRedis(string method, string key = "", string member = "")
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType   = EumCallType.Redis,
                CallMethod = method,
                Data = new PooledDictionary<string, string>
                {
                    { "RedisKey", key },
                    { "RedisHashFields", member }
                }
            };
            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        ///     追踪Elasticsearch
        /// </summary>
        internal static TrackEnd TrackElasticsearch(string method)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType   = EumCallType.Elasticsearch,
                CallMethod = method
            };
            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        ///     追踪Mq
        /// </summary>
        public static TrackEnd TrackMqProduct(string method)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType   = EumCallType.Mq,
                CallMethod = method
            };
            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        ///     追踪Grpc
        /// </summary>
        internal static TrackEnd TrackGrpc(string server, string action)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType = EumCallType.GrpcClient,
                StartTs  = DateTime.Now.ToTimestamps(),
                Data = new PooledDictionary<string, string>
                {
                    { "Server", server },
                    { "Action", action }
                }
            };
            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        ///     追踪Http
        /// </summary>
        internal static TrackEnd TrackHttp(string url, string method, IDictionary<string, string> headerData, string requestBody)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType = EumCallType.HttpClient,
                StartTs  = DateTime.Now.ToTimestamps(),
                Data = new PooledDictionary<string, string>
                {
                    { "Url", url },
                    { "Method", method },
                    { "RequestBody", requestBody },
                    { "Header", headerData != null ? JsonConvert.SerializeObject(value: headerData) : "{}" }
                }
            };
            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        ///     关键位置埋点
        /// </summary>
        internal static TrackEnd TrackKeyLocation(string message)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType = EumCallType.KeyLocation,
                StartTs  = DateTime.Now.ToTimestamps(),
                Data = new PooledDictionary<string, string>
                {
                    { "Message", message }
                }
            };
            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        ///     手动埋点
        /// </summary>
        public static TrackEnd Track(string message)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType = EumCallType.Custom,
                StartTs  = DateTime.Now.ToTimestamps(),
                Data = new PooledDictionary<string, string>
                {
                    { "Message", message }
                }
            };
            Current.Set(linkTrackDetail: linkTrackDetail);
            return new TrackEnd(linkTrackDetail: linkTrackDetail);
        }

        /// <summary>
        /// 记录异常信息
        /// </summary>
        public static void Exception(string method, Dictionary<string, string> methodParams, string exceptionTypeName, string exceptionMessage, string callStacks)
        {
            var linkTrackContext = Current.Get();
            // 说明是异常发生处
            if (linkTrackContext.ExceptionDetail == null)
            {
                linkTrackContext.ExceptionDetail = new()
                {
                    Method            = method,
                    MethodParams      = methodParams,
                    ExceptionTypeName = exceptionTypeName,
                    ExceptionMessage  = exceptionMessage,
                    CallStacks        = new List<string>() { callStacks },
                    CreateAt          = DateTime.Now.ToTimestamps(),
                };
            }
            else
            {
                // 之前已记录了异常发生处，再次传入，说明是调用堆栈的信息
                linkTrackContext.ExceptionDetail.CallStacks.Add(callStacks);
            }
        }
    }
}