using System.Collections.Generic;

namespace FS.Core.LinkTrack
{
    public class LinkTrackContext
    {
        /// <summary>
        /// 应用
        /// </summary>
        public virtual string AppId { get; set; }
        
        /// <summary>
        /// 上下文ID
        /// </summary>
        public virtual string ContextId { get; set; }

        /// <summary>
        /// 调用开始时间戳
        /// </summary>
        public virtual long StartTs { get; set; }

        /// <summary>
        /// 调用的上下文
        /// </summary>
        public virtual List<LinkTrackDetail> List { get; set; } = new();
    }
}