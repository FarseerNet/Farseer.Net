using FS.Core.LinkTrack;
using Nest;

namespace FS.LinkTrack.Dal
{
    /// <summary>
    /// 慢查询
    /// </summary>
    public class SlowQueryPO
    {
        /// <summary>
        ///     上下文ID
        /// </summary>
        [Keyword]
        public string ContextId { get; set; }

        /// <summary>
        ///     应用
        /// </summary>
        [Keyword]
        public string AppName { get; set; }
        /// <summary>
        ///     应用Id
        /// </summary>
        public long AppId { get; set; }

        /// <summary>
        ///     应用Ip
        /// </summary>
        [Keyword]
        public string AppIp { get; set; }

        /// <summary>
        ///     调用类型
        /// </summary>
        [Number(type: NumberType.Byte)]
        public EumCallType CallType { get; set; }

        /// <summary>
        ///     总共使用时间毫秒
        /// </summary>
        [Number(type: NumberType.Long)]
        public long UseTs { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        [Number(type: NumberType.Long)]
        public long StartTs { get; set; }

        /// <summary>
        ///     数据库名称
        /// </summary>
        [Keyword]
        public string DbName { get; set; }
        /// <summary>
        ///     表名称
        /// </summary>
        [Keyword]
        public string DbTableName { get; set; }
        /// <summary>
        ///     SQL文本
        /// </summary>
        [Text]
        public string DbSql { get; set; }

        /// <summary>
        ///     Redis key
        /// </summary>
        [Keyword]
        public string RedisKey { get; set; }
        /// <summary>
        ///     Redis hash key
        /// </summary>
        [Keyword]
        public string RedisHashFields { get; set; }

        /// <summary>
        ///     Elasticsearch调用方法
        /// </summary>
        [Keyword]
        public string EsMethod { get; set; }

        /// <summary>
        ///     Mq Topic
        /// </summary>
        [Keyword]
        public string MqTopic { get; set; }

        /// <summary>
        ///     Grpc
        /// </summary>
        [Keyword]
        public string GrpcUrl { get; set; }

        /// <summary>
        ///     http url
        /// </summary>
        [Keyword]
        public string HttpUrl { get; set; }
        /// <summary>
        ///     http Method
        /// </summary>
        [Keyword]
        public string HttpMethod { get; set; }
        /// <summary>
        ///     http RequestBody
        /// </summary>
        [Keyword]
        public string HttpRequestBody { get; set; }
    }
}