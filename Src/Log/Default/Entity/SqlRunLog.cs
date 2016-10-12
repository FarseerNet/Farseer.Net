using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml.Serialization;
using FS.Utils.Common;

namespace FS.Log.Default.Entity
{
    /// <summary> SQL执行记录 </summary>
    [Serializable]
    public class SqlRunLog : CommonLog
    {
        /// <summary>
        /// 日志写入器
        /// </summary>
        private static readonly LogWrite Writer = new LogWrite(EumLogType.Info, SysMapPath.SqlRunPath, 30);
        /// <summary> 执行对象 </summary>
        public CommandType CmdType { get; set; }

        /// <summary> 执行时间（毫秒） </summary>
        public long UserTime { get; set; }

        /// <summary> 执行表名称 </summary>
        public string Name { get; set; }

        /// <summary> 执行SQL </summary>
        public string Sql { get; set; }

        /// <summary> 执行参数 </summary>
        [XmlElement]
        public List<SqlParam> SqlParamList { get; set; }

        /// <summary> SQL执行记录</summary>
        /// <param name="name">表名称</param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        /// <param name="elapsedMilliseconds">执行时间（单位：ms）</param>
        public SqlRunLog(string name, CommandType cmdType, string sql, IEnumerable<DbParameter> param, long elapsedMilliseconds)
        {
            Name = name;
            CmdType = cmdType;
            Sql = sql ?? "";
            UserTime = elapsedMilliseconds;
            SqlParamList = new List<SqlParam>();
            foreach (var dbParameter in param)
            {
                SqlParamList.Add(new SqlParam { Name = dbParameter.ParameterName, Value = dbParameter.Value.ToString() });
            }
        }

        public void Write() => Writer.Add(this);
    }
}