using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Data.Infrastructure;

namespace FS.Data.Internal
{
    /// <summary>
    ///     存储SQL及SQL参数
    /// </summary>
    public class SqlParam : ISqlParam
    {
        public SqlParam(string sql, params DbParameter[] parameters)
        {
            Sql = new StringBuilder(sql);
            if (parameters != null) { Param = parameters.ToList(); }
        }

        public string Name { get; private set; }
        public StringBuilder Sql { get; private set; }
        public List<DbParameter> Param { get; }

        #region 释放

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                Sql.Clear();
                Sql = null;
                Param?.Clear();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}