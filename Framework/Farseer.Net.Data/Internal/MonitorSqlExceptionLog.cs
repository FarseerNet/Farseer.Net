using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using FS.Core;
using FS.Data.Log.Default;

namespace FS.Data.Internal
{
    /// <summary>
    ///     数据库操作（支持异常SQL日志记录）
    /// </summary>
    internal class MonitorSqlExceptionLog : ISqlMonitor
    {

        public void ExecuteException(string methodName, string name, CommandType cmdType, string sql, IEnumerable<DbParameter> param, long elapsedMilliseconds, Exception exp) => new SqlErrorLog(exp, name, CommandType.Text, sql, (List<DbParameter>)param ?? new List<DbParameter>()).Write();

        public void PreExecute(string methodName, string name, CommandType cmdType, string sql, IEnumerable<DbParameter> param)
        {
        }

        public void Executed<TReturn>(string methodName, string name, CommandType cmdType, string sql, IEnumerable<DbParameter> param, long elapsedMilliseconds, TReturn result)
        {
        }
    }
}