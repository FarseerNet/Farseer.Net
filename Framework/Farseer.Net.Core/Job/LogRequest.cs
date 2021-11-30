using System;
using Microsoft.Extensions.Logging;

namespace FS.Core.Job
{
    /// <summary>
    ///     客户端请求过来的日志内容
    /// </summary>
    public class LogRequest
    {
        /// <summary>
        ///     日志等级
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        ///     日志内容
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        ///     记录时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}