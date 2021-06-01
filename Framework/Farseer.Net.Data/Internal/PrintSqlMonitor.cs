using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using FS.Core;
using FS.Data.Log.Default;

namespace FS.Data.Internal
{
    /// <summary>
    ///     将SQL发送到数据库（代理类、记录SQL、执行时间）
    /// </summary>
    internal class PrintSqlMonitor : ISqlMonitor
    {
        public void ExecuteException(string methodName, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param, long elapsedMilliseconds, Exception exp)
        {
            new SqlErrorLog(exp, dbName, tableName, CommandType.Text, sql, (List<DbParameter>) param ?? new List<DbParameter>()).Print();
        }

        public void PreExecute(string methodName, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param)
        {
        }

        public void Executed<TReturn>(string methodName, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param, long elapsedMilliseconds, TReturn result)
        {
            new SqlRunLog(dbName, tableName, cmdType, sql, param, elapsedMilliseconds).Print();
        }
    }
}