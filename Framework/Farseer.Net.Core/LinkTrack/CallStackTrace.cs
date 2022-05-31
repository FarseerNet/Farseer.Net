using System.Collections.Generic;
using Collections.Pooled;

namespace FS.Core.LinkTrack
{
    public class CallStackTrace
    {
        /// <summary>
        ///     调用方法
        /// </summary>
        public string CallMethod { get; set; }

        /// <summary>
        ///     执行文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     方法执行行数
        /// </summary>
        public int FileLineNumber { get; set; }

        /// <summary>
        ///     方法返回类型
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>
        ///     方法入参
        /// </summary>
        public PooledDictionary<string, string> MethodParams { get; set; }
    }
}