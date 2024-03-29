﻿using System;
using System.Data.Common;
using System.Text;
using Collections.Pooled;
using FS.Data.Map;

namespace FS.Data.Abstract
{
    /// <summary>
    ///     SQL语句与参数
    /// </summary>
    public interface ISqlParam : IDisposable
    {
        /// <summary>
        ///     当前生成的SQL语句
        /// </summary>
        StringBuilder Sql { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        PooledList<DbParameter> Param { get; }
        /// <summary>
        /// 实体类结构映射
        /// </summary>
        SetDataMap SetMap { get; }
    }
}