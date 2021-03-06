using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using FS.Extends;

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
        public LinkTrackContext Get() => AsyncLocal.Value ??= new LinkTrackContext()
        {
            AppId       = Assembly.GetEntryAssembly().FullName.Split(',')[0].ToLower(),
            ParentAppId = "",
            ContextId   = SnowflakeId.GenerateId.ToString(),
            StartTs     = DateTime.Now.ToTimestamps(),
            List        = new List<LinkTrackDetail>()
        };

        /// <summary>
        ///     写入上下文ID
        /// </summary>
        public void Set(string contextId, string parentAppId) => AsyncLocal.Value = new LinkTrackContext()
        {
            AppId       = Assembly.GetEntryAssembly().FullName.Split(',')[0].ToLower(),
            ParentAppId = parentAppId,
            ContextId   = contextId,
            StartTs     = DateTime.Now.ToTimestamps(),
            List        = new List<LinkTrackDetail>()
        };

        /// <summary>
        ///     写入数据
        /// </summary>
        public void Set(LinkTrackDetail linkTrackDetail) => Get().List.Add(linkTrackDetail);

        /// <summary>
        /// 追踪数据库
        /// </summary>
        public static TrackEnd TrackDatabase(string method, DbLinkTrackDetail dbLinkTrackDetail = null)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType          = EumCallType.Database,
                DbLinkTrackDetail = dbLinkTrackDetail,
                CallMethod        = method
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }

        /// <summary>
        /// 追踪数据库
        /// </summary>
        public static TrackEnd TrackDatabase(string method, string connectionString)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType          = EumCallType.Database,
                DbLinkTrackDetail = new DbLinkTrackDetail() {ConnectionString = connectionString},
                CallMethod        = method
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }

        /// <summary>
        /// 追踪数据库
        /// </summary>
        public static TrackEnd TrackDatabase(string method, string connectionString,string tableName)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType          = EumCallType.Database,
                DbLinkTrackDetail = new DbLinkTrackDetail() {ConnectionString = connectionString,TableName = tableName},
                CallMethod        = method
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }

        /// <summary>
        /// 追踪数据库
        /// </summary>
        public static TrackEnd TrackDatabase(string method, string connectionString, CommandType commandType, string sql, params DbParameter[] parameters)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType = EumCallType.Database,
                DbLinkTrackDetail = new DbLinkTrackDetail()
                {
                    ConnectionString = connectionString,
                    CommandType      = commandType,
                    Sql              = sql,
                    SqlParam         = parameters.ToDictionary(o => o.ParameterName, o => o.Value.ToString())
                },
                CallMethod = method
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }

        /// <summary>
        /// 追踪Redis
        /// </summary>
        public static TrackEnd TrackApiServer(string domain, string path, string method, string contentType, Dictionary<string, string> headerDictionary, string requestBody, string ip)
        {
            var linkTrackContext = Current.Get();
            linkTrackContext.Domain      = domain;
            linkTrackContext.Path        = path;
            linkTrackContext.Method      = method;
            linkTrackContext.ContentType = contentType;
            linkTrackContext.Headers     = headerDictionary;
            linkTrackContext.RequestBody = requestBody;
            linkTrackContext.RequestIp   = ip;
            return new TrackEnd(linkTrackContext);
        }

        /// <summary>
        /// 追踪Redis
        /// </summary>
        public static TrackEnd TrackRedis(string method, string key = "", string member = "")
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType   = EumCallType.Redis,
                CallMethod = method,
                Data = new Dictionary<string, string>()
                {
                    {"RedisKey", key},
                    {"RedisHashFields", member},
                }
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }

        /// <summary>
        /// 追踪Elasticsearch
        /// </summary>
        public static TrackEnd TrackElasticsearch(string method)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType   = EumCallType.Elasticsearch,
                CallMethod = method
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }

        /// <summary>
        /// 追踪Mq
        /// </summary>
        public static TrackEnd TrackMq(string method)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType   = EumCallType.Mq,
                CallMethod = method
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }

        /// <summary>
        /// 追踪Grpc
        /// </summary>
        public static TrackEnd TrackGrpc(string server, string action)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType = EumCallType.GrpcClient,
                StartTs  = DateTime.Now.ToTimestamps(),
                Data = new Dictionary<string, string>()
                {
                    {"Server", server},
                    {"Action", action},
                }
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }

        /// <summary>
        /// 手动埋点
        /// </summary>
        public static TrackEnd Track(string message)
        {
            var linkTrackDetail = new LinkTrackDetail
            {
                CallType = EumCallType.Custom,
                StartTs  = DateTime.Now.ToTimestamps(),
                Data = new Dictionary<string, string>()
                {
                    {"Message", message}
                }
            };
            Current.Set(linkTrackDetail);
            return new TrackEnd(linkTrackDetail);
        }
    }
}