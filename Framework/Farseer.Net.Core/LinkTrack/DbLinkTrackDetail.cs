using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace FS.Core.LinkTrack
{
    public class DbLinkTrackDetail
    {
        /// <summary>
        ///     连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     数据库名称
        /// </summary>
        public string DataBaseName { get; set; }

        /// <summary>
        ///     表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///     执行方式
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        ///     SQL文本
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 设置SQL入参
        /// </summary>
        /// <param name="param"></param>
        public void SetDbParam(IEnumerable<DbParameter> param)
        {
            if (param != null && param.Any())
            {
                foreach (var sqlParam in param)
                {
                    Sql = Sql.Replace(sqlParam.ParameterName, $"\"{sqlParam.Value}\"");
                }
            }
        }
    }
}