using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Collections.Pooled;
using FS.Utils.Common;

namespace FS.Data.Log.Default.Entity
{
    /// <summary>
    ///     日志记录
    /// </summary>
    [DataContract(Name = "Log")]
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

        /// <summary> 数据库名称 </summary>
        [DataMember]
        public string DbName { get; set; }

        /// <summary> 执行表名称 </summary>
        [DataMember]
        public string TableName { get; set; }

        /// <summary> 执行SQL </summary>
        [DataMember]
        public string Sql { get; set; }

        /// <summary> 执行对象 </summary>
        [DataMember]
        public CommandType CmdType { get; set; }

        /// <summary> 执行参数 </summary>
        [DataMember]
        public PooledList<SqlParam> SqlParamList { get; set; }

        /// <summary>
        ///     记录执行时的方法及文件
        /// </summary>
        internal void RecordExecuteMethod()
        {
            CreateAt = DateTime.Now.ToString(format: "yyyy-MM-dd HH:mm:ss");

            var lstFrames = Exp == null ? new StackTrace(fNeedFileInfo: true).GetFrames() : new StackTrace(e: Exp, fNeedFileInfo: true).GetFrames();
            var stack     = lstFrames?.LastOrDefault(predicate: o => o.GetFileLineNumber() != 0 && !o.GetMethod().Module.Name.Contains(value: "Farseer.Net") && !StringHelper.IsEquals(str: o.GetMethod().Name, str2: "Callback"));
            if (stack == null) return;

            LineNo     = stack.GetFileLineNumber();
            MethodName = stack.GetMethod().Name;
            FileName   = stack.GetFileName();
        }
    }
}