﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FS.Data.Infrastructure
{
    /// <summary>
    ///     SQL语句与参数
    /// </summary>
    public interface ISqlParam : IDisposable
    {
        /// <summary>
        ///     数据库名称
        /// </summary>
        string DbName { get; }

        /// <summary>
        ///     表名/视图名
        /// </summary>
        string TableName { get; }

        /// <summary>
        ///     当前生成的SQL语句
        /// </summary>
        StringBuilder Sql { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        List<DbParameter> Param { get; }
    }
}