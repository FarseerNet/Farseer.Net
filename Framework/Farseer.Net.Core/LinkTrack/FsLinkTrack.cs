using System;
using System.Collections.Generic;
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
            AppId     = Assembly.GetEntryAssembly().FullName.Split(',')[0],
            ContextId = SnowflakeId.GenerateId.ToString(),
            StartTs   = DateTime.Now.ToTimestamps(),
            List      = new List<LinkTrackDetail>()
        };

        /// <summary>
        ///     写入上下文ID
        /// </summary>
        public void Set(string contextId) => AsyncLocal.Value = new LinkTrackContext()
        {
            AppId     = Assembly.GetEntryAssembly().FullName.Split(',')[0],
            ContextId = contextId,
            StartTs   = DateTime.Now.ToTimestamps(),
            List      = new List<LinkTrackDetail>()
        };

        /// <summary>
        ///     写入数据
        /// </summary>
        public void Set(LinkTrackDetail linkTrackDetail) => Get().List.Add(linkTrackDetail);

        /// <summary>
        /// 追踪Redis
        /// </summary>
        public static void TrackRedis(string method)
        {
            Current.Set(new LinkTrackDetail
            {
                CallType   = EumCallType.Redis,
                CallMethod = method
            });
        }

        /// <summary>
        /// 追踪Elasticsearch
        /// </summary>
        public static void TrackElasticsearch(string method)
        {
            Current.Set(new LinkTrackDetail
            {
                CallType   = EumCallType.Elasticsearch,
                CallMethod = method
            });
        }

        /// <summary>
        /// 追踪Mq
        /// </summary>
        public static void TrackMq(string method)
        {
            Current.Set(new LinkTrackDetail
            {
                CallType   = EumCallType.Mq,
                CallMethod = method
            });
        }
    }
}