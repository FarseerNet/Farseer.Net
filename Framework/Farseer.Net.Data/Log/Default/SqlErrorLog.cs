using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;
using Farseer.Net.Configuration;
using Farseer.Net.Data.Log.Default.Entity;

namespace Farseer.Net.Data.Log.Default
{
    /// <summary> SQL异常记录 </summary>
    [DataContract]
    public class SqlErrorLog : CommonLog
    {
        /// <summary>
        /// 日志写入器
        /// </summary>
        private static readonly LogWrite Writer = new LogWrite(EumLogType.Error, SysPath.SqlErrorPath, 5);

        /// <summary> 执行对象 </summary>
        [DataMember]
        public CommandType CmdType { get; set; }

        /// <summary> 执行表名称 </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary> 执行SQL </summary>
        [DataMember]
        public string Sql { get; set; }

        /// <summary> 执行参数 </summary>
        [DataMember]
        public List<SqlParam> SqlParamList { get; set; }

        /// <summary> SQL异常记录写入 </summary>
        /// <param name="name">表名称</param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        /// <param name="exp">异常信息</param>
        public SqlErrorLog(Exception exp, string name, CommandType cmdType, string sql, List<DbParameter> param)
        {
            Exp = exp;
            Message = exp.Message.Replace("\r\n", "");
            Name = name;
            CmdType = cmdType;
            Sql = sql;
            if (param != null && param.Count > 0)
            {
                SqlParamList = new List<SqlParam>();
                foreach (var t in param)
                {
                    SqlParamList.Add(new SqlParam { Name = t.ParameterName, Value = (t.Value ?? "null").ToString() });
                }
            }
            RecordExecuteMethod();
        }

        public void Write()
        {
            Writer.Add(this);
        }
    }
}