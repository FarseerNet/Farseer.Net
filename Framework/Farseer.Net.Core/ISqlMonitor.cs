using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace FS.Core
{
    /// <summary>
    /// SQL执行监控
    /// </summary>
    public interface ISqlMonitor
    {
        /// <summary>
        /// 监控执行结果
        /// </summary>
        /// <param name="methodName">动作</param>
        /// <param name="dbName">数据库名称 </param>
        /// <param name="tableName">表名称</param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        /// <param name="elapsedMilliseconds">执行时间（单位：ms）</param>
        /// <param name="exp">异常</param>
        void ExecuteException(string methodName, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param, long elapsedMilliseconds, System.Exception exp);

        /// <summary>
        /// 监控执行结果（执行前）
        /// </summary>
        /// <param name="methodName">动作</param>
        /// <param name="tableName">表名称</param>
        /// <param name="dbName">数据库名称 </param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        void PreExecute(string methodName, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param);

        /// <summary>
        /// 监控执行结果（执行后）
        /// </summary>
        /// <param name="methodName">动作</param>
        /// <param name="tableName">表名称</param>
        /// <param name="dbName">数据库名称 </param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        /// <param name="elapsedMilliseconds">执行时间（单位：ms）</param>
        /// <param name="result">返回结果</param>
        void Executed<TReturn>(string methodName, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param, long elapsedMilliseconds, TReturn result);
    }
}