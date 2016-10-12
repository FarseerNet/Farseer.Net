using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Log.Default;
using FS.Utils.Common;

namespace FS.Log
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public static class LogManger
    {
        /// <summary>
        /// 日志管理
        /// </summary>
        public static ILog Log { get; private set; } = new DefaultLog();

        /// <summary>
        /// 使用外部实现方式
        /// </summary>
        /// <param name="log"></param>
        public static void Set(ILog log)
        {
            Log = log;
        }
    }
}
