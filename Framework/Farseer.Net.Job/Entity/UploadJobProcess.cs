using System;
using FSS.GrpcService;

namespace FS.Job.Entity
{
    /// <summary>
    /// 用于上传当前JOB的执行进度
    /// </summary>
    public class UploadJobProgress
    {
        /// <summary>
        /// 日志
        /// </summary>
        public LogResponse Log { get; set; }
        
        /// <summary>
        /// 当前运行速度
        /// </summary>
        public int RunSpeed { get; set; }

        /// <summary>
        /// 下次执行时间
        /// </summary>
        public long NextAt { get; set; }

        /// <summary>
        /// 当前进度
        /// </summary>
        public int Progress { get; internal set; }
    }
}