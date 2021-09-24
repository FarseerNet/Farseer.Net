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
    /// <summary> SQL执行记录 </summary>
    [DataContract]
    public class SqlRunLog : CommonLog
    {
        /// <summary>
        ///     日志写入器
        /// </summary>
        private static readonly LogWrite Writer = new(logType: EumLogType.Info, filePath: SysPath.SqlRunPath, lazyTime: 30);

        /// <summary> SQL执行记录 </summary>
        /// <param name="dbName"> 数据库名称 </param>
        /// <param name="tableName"> 表名称 </param>
        /// <param name="cmdType"> 执行方式 </param>
        /// <param name="sql"> T-SQL </param>
        /// <param name="param"> SQL参数 </param>
        /// <param name="elapsedMilliseconds"> 执行时间（单位：ms） </param>
        public SqlRunLog(string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param, long elapsedMilliseconds)
        {
            DbName       = dbName;
            TableName    = tableName;
            CmdType      = cmdType;
            Sql          = sql ?? "";
            UserTime     = elapsedMilliseconds;
            SqlParamList = new List<SqlParam>();
            foreach (var dbParameter in param) SqlParamList.Add(item: new SqlParam { Name = dbParameter.ParameterName, Value = dbParameter.Value.ToString() });
            RecordExecuteMethod();
        }

        /// <summary> 执行时间（毫秒） </summary>
        [DataMember]
        public long UserTime { get; set; }

        public void Write() => Writer.Add(obj: this);

        /// <summary>
        ///     打印日志
        /// </summary>
        public void Print()
        {
            IocManager.Instance.Logger<SqlRunLog>().LogInformation(message: $"db={DbName},table={TableName},耗时={UserTime},cmd={CmdType.ToString()},sql={Sql},sqlParam={string.Join(separator: "|", values: SqlParamList.Select(selector: o => $"{o.Name}={o.Value}"))}");
        }
    }
}