using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml.Serialization;
using FS.Utils.Common;

namespace FS.Log.Entity
{
    /// <summary> SQL执行记录 </summary>
    [Serializable]
    public class SqlRunLogEntity : AbsLogEntity<SqlRunLogEntity>
    {
        public SqlRunLogEntity() : base(eumLogType.Debug, null, null, 0) { }
        /// <summary> SQL执行记录写入~/App_Data/SqlLog.xml </summary>
        /// <param name="name">表名称</param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        /// <param name="elapsedMilliseconds">执行时间（单位：ms）</param>
        public SqlRunLogEntity(string name, CommandType cmdType, string sql, List<DbParameter> param, long elapsedMilliseconds) : base(eumLogType.Info, SysMapPath.SqlRunPath, $"{DateTime.Now.ToString("yy-MM-dd")}.xml", 1)
        {
            Name = name;
            CmdType = cmdType;
            SqlParamList = new List<SqlParam>();
            Sql = sql ?? "";
            UserTime = elapsedMilliseconds;
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
        [XmlElement]
        public List<SqlParam> SqlParamList { get; set; }

        public override void AddToQueue()
        {
            //写入日志
            AddToQueue(this);
        }
    }
}