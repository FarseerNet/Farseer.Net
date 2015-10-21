using System;
using System.Diagnostics;
using System.Linq;
using FS.Utils.Common;

namespace FS.Log
{
    /// <summary>
    ///     日志记录
    /// </summary>
    public abstract class AbsLogEntity
    {
        /// <summary> 执行行数 </summary>
        public int LineNo { get; set; }

        /// <summary> 执行方法名称 </summary>
        public string MethodName { get; set; }

        /// <summary> 执行方法的文件名 </summary>
        public string FileName { get; set; }

        /// <summary> 执行时间 </summary>
        public DateTime CreateAt { get; set; }

        /// <summary> 执行时间 </summary>
        protected Exception ex { get; set; }

        /// <summary>
        ///     记录执行时的方法及文件
        /// </summary>
        protected void RecordExecuteMethod()
        {
            CreateAt = DateTime.Now;

            var lstFrames = ex == null ? new StackTrace(true).GetFrames() : new StackTrace(ex, true).GetFrames();
            var stack = lstFrames?.LastOrDefault(o => o.GetFileLineNumber() != 0 && !ConvertHelper.IsEquals(o.GetMethod().Module.Name, "Farseer.Net.dll") && !ConvertHelper.IsEquals(o.GetMethod().Name, "Callback"));
            if (stack == null) return;

            LineNo = stack.GetFileLineNumber();
            MethodName = stack.GetMethod().Name;
            FileName = stack.GetFileName();
        }
    }
}