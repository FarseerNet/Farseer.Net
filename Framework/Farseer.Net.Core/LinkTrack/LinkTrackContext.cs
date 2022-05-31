using System;
using System.Collections.Generic;
using Collections.Pooled;

namespace FS.Core.LinkTrack
{
    public class LinkTrackContext : IDisposable
    {
        /// <summary>
        ///     上游应用
        /// </summary>
        public virtual string ParentAppName { get; set; }

        /// <summary>
        ///     上下文ID
        /// </summary>
        public virtual string ContextId { get; set; }

        /// <summary>
        ///     调用开始时间戳
        /// </summary>
        public virtual long StartTs { get; set; }

        /// <summary>
        ///     调用结束时间戳
        /// </summary>
        public virtual long EndTs { get; set; }

        /// <summary>
        ///     总共使用时间毫秒
        /// </summary>
        public virtual long UseTs => EndTs > StartTs ? EndTs - StartTs : 0;

        /// <summary>
        /// 状态码
        /// </summary>
        public virtual EumLinkType LinkType { get; set; }

        /// <summary>
        ///     请求域名
        /// </summary>
        public virtual string Domain { get; set; }

        /// <summary>
        ///     请求地址
        /// </summary>
        public virtual string Path { get; set; }

        /// <summary>
        ///     请求方式
        /// </summary>
        public virtual string Method { get; set; }

        /// <summary>
        ///     请求内容类型
        /// </summary>
        public virtual string ContentType { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public virtual string StatusCode { get; set; }

        /// <summary>
        ///     请求头部
        /// </summary>
        public virtual PooledDictionary<string, string> Headers { get; set; }

        /// <summary>
        ///     请求参数
        /// </summary>
        public virtual string RequestBody { get; set; }

        /// <summary>
        ///     输出参数
        /// </summary>
        public virtual string ResponseBody { get; set; }

        /// <summary>
        ///     客户端IP
        /// </summary>
        public virtual string RequestIp { get; set; }

        /// <summary>
        ///     调用的上下文
        /// </summary>
        public virtual PooledList<LinkTrackDetail> List { get; set; } = new();

        /// <summary>
        ///     是否执行异常
        /// </summary>
        public virtual bool IsException { get; set; }

        /// <summary>
        ///     异常信息
        /// </summary>
        public virtual string ExceptionMessage { get; set; }
        public void Dispose()
        {
            if (Headers != null) Headers.Dispose();
            if (List != null)
            {
                foreach (var linkTrackDetail in List)
                {
                    linkTrackDetail.Data?.Dispose();
                    linkTrackDetail.CallStackTrace?.MethodParams.Dispose();
                }
            }
        }
    }
}