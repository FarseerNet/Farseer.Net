using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using FS.Utils.Common;

namespace FS.Log.Default.Entity
{
    /// <summary>
    ///     日志记录
    /// </summary>
    [DataContract(Name ="Log")]
    public class CommonLog
    {
        /// <summary> 执行行数 </summary>
        [DataMember]
        public int LineNo { get; set; }

        /// <summary> 执行方法名称 </summary>
        [DataMember]
        public string MethodName { get; set; }

        /// <summary> 执行方法的文件名 </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary> 执行时间 </summary>
        [DataMember]
        public string CreateAt { get; set; }

        /// <summary> 执行时间 </summary>
        public Exception Exp { get; set; }

        /// <summary> 异常消息 </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        ///     记录执行时的方法及文件
        /// </summary>
        internal void RecordExecuteMethod()
        {
            CreateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var lstFrames = Exp == null ? new StackTrace(true).GetFrames() : new StackTrace(Exp, true).GetFrames();
            var stack = lstFrames?.LastOrDefault(o => o.GetFileLineNumber() != 0 && !o.GetMethod().Module.Name.Contains("Farseer.Net") && !ConvertHelper.IsEquals(o.GetMethod().Name, "Callback"));
            if (stack == null) return;

            LineNo = stack.GetFileLineNumber();
            MethodName = stack.GetMethod().Name;
            FileName = stack.GetFileName();
        }
    }
}