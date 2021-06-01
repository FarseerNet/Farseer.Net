using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using FS.Configuration;
using FS.Data.Log.Default.Entity;
using FS.DI;
using Microsoft.Extensions.Logging;

namespace FS.Data.Log.Default
{
    /// <summary> SQL异常记录 </summary>
    [DataContract]
    public class SqlErrorLog : CommonLog
    {
        /// <summary>
        /// 日志写入器
        /// </summary>
        private static readonly LogWrite Writer = new LogWrite(EumLogType.Error, SysPath.SqlErrorPath, 5);

        /// <summary> SQL异常记录写入 </summary>
        /// <param name="dbName">数据库名称 </param>
        /// <param name="tableName">表名称</param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        /// <param name="exp">异常信息</param>
        public SqlErrorLog(Exception exp,string dbName, string tableName, CommandType cmdType, string sql, List<DbParameter> param)
        {
            Exp = exp;
            Message = exp.Message.Replace("\r\n", "");
            DbName = dbName;
            TableName = tableName;
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

        /// <summary>
        /// 打印日志
        /// </summary>
        public void Print()
        {
            IocManager.Instance.Logger<SqlRunLog>().LogError($"db={DbName},table={TableName},cmd={CmdType.ToString()},sql={Sql},sqlParam={string.Join("|", SqlParamList.Select(o=>$"{o.Name}={o.Value}"))}");
        }
    }
}