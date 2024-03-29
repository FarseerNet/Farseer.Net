﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Collections.Pooled;
using FS.Data.Abstract;
using FS.Data.Map;

namespace FS.Data.Internal
{
    /// <summary>
    ///     存储SQL及SQL参数
    /// </summary>
    public class SqlParam : ISqlParam
    {
        public SqlParam(string sql, IEnumerable<DbParameter> parameters)
        {
            Sql = new StringBuilder(value: sql);
            if (parameters != null) Param = parameters.ToPooledList();
        }

        public SqlParam(IProcParam procParam)
        {
            Param  = procParam.Param;
            SetMap = procParam.SetMap;
        }

        public StringBuilder           Sql    { get; private set; }
        public PooledList<DbParameter> Param  { get; }
        public SetDataMap              SetMap { get; }

        #region 释放

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing"> 是否释放托管资源 </param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                Sql.Clear();
                Sql = null;
                Param?.Dispose();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        #endregion
    }
}