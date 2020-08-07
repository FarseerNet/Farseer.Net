using System;
using System.Threading;

namespace FS.Context
{
    /// <summary>
    /// Trace上下文,仅限.net4.5及以上
    /// </summary>
    public class TraceIdContext
    {
        /// <summary>
        /// TraceId
        /// </summary>
        public string TraceId { get; set; }
        /// <summary>
        /// 根
        /// </summary>
        public string RootId { get; set; }
        /// <summary>
        /// 父
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 当前
        /// </summary>
        public string ChildId { get; set; }

        /// <summary>
        /// 如果不传入,则自动生成一个 GUID
        /// </summary>
        /// <param name="traceId"></param>
        public TraceIdContext(string traceId)
        {
            if (string.IsNullOrEmpty(traceId))
            {
                traceId = Guid.NewGuid().ToString();
            }
            this.TraceId = traceId;

        }


        public TraceIdContext(string traceId, string rootId, string parentId, string childId)
        {
            this.TraceId = traceId;
            this.RootId = rootId;
            this.ParentId = parentId;
            this.ChildId = childId;
        }
        private static readonly AsyncLocal<TraceIdContext> HttpContextCurrent = new AsyncLocal<TraceIdContext>();
        public static TraceIdContext Current
        {
            get => HttpContextCurrent.Value;
            set => HttpContextCurrent.Value = value;
        }
    }
}
