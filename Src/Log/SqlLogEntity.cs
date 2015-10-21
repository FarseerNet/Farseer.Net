using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using FS.Utils.Common;

namespace FS.Log
{
    /// <summary> SQL执行记录 </summary>
    [Serializable]
    public class SqlLogEntity : AbsLogEntity
    {
        /// <summary> SQL执行记录管理器 </summary>
        private static readonly CommonLogManger<SqlLogEntity> LogManger = new CommonLogManger<SqlLogEntity>(SysMapPath.AppData + "log/", "SqlLog.xml", 1);

        public SqlLogEntity()
        {
        }

        /// <summary> SQL执行记录写入~/App_Data/SqlLog.xml </summary>
        /// <param name="name">表名称</param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        /// <param name="elapsedMilliseconds">执行时间（单位：ms）</param>
        public static void Write(string name, CommandType cmdType, string sql, List<DbParameter> param, long elapsedMilliseconds)
        {
            var entity = new SqlLogEntity {Name = name, CmdType = cmdType, SqlParamList = new List<SqlParam>(), Sql = sql ?? "",};
            if (param != null && param.Count > 0) { param.ForEach(o => entity.SqlParamList.Add(new SqlParam {Name = o.ParameterName, Value = (o.Value ?? "null").ToString()})); }
            entity.Write(elapsedMilliseconds);
        }

        /// <summary> 执行对象 </summary>
        public CommandType CmdType { get; set; }

        /// <summary> 执行时间（毫秒） </summary>
        public long UserTime { get; set; }

        /// <summary> 执行表名称 </summary>
        public string Name { get; set; }

        /// <summary> 执行SQL </summary>
        public string Sql { get; set; }

        /// <summary> 执行参数 </summary>
        public List<SqlParam> SqlParamList { get; set; }

        /// <summary> 写入~/App_Data/SqlLog.xml </summary>
        /// <param name="elapsedMilliseconds">执行耗时</param>
        private void Write(long elapsedMilliseconds)
        {
            RecordExecuteMethod();
            UserTime = elapsedMilliseconds;
            LogManger.Write(this);
        }
    }
}