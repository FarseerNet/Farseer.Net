using System.Collections.Generic;
using FS.Core.Abstract.Fss;

namespace FS.Fss.Entity
{
    /// <summary>
    ///     客户端执行情况
    /// </summary>
    public class JobInvokeRequest
    {
        /// <summary>
        ///     任务组ID
        /// </summary>
        public int TaskGroupId { get; set; }

        /// <summary>
        ///     下次执行时间
        /// </summary>
        public long NextTimespan { get; set; }

        /// <summary>
        ///     当前进度
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        ///     执行状态
        /// </summary>
        public EumTaskType Status { get; set; }

        /// <summary>
        ///     执行速度
        /// </summary>
        public long RunSpeed { get; set; }

        /// <summary>
        ///     日志
        /// </summary>
        public LogRequest Log { get; set; }

        /// <summary>
        ///     数据
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
    }
}