using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Utils.Common;

namespace FS.Log
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public class LogManger
    {
        private static ILog _log;
        /// <summary>
        /// 日志管理
        /// </summary>
        public static ILog Log => _log ?? (_log = new DefaultLog());

        public static void Set(ILog log)
        {
            _log = log;
        }
    }
}
