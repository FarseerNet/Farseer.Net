using System;
using System.Data.Common;
using Collections.Pooled;
using FS.Data.Map;

namespace FS.Data.Abstract
{
    /// <summary>
    ///     SQL参数
    /// </summary>
    public interface IProcParam : IDisposable
    {
        /// <summary>
        /// 实体类结构映射
        /// </summary>
        public SetDataMap SetMap { get; }

        /// <summary>
        ///     存储过程名
        /// </summary>
        string ProcName { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        PooledList<DbParameter> Param { get; }
    }
}