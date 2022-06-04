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
        /// <summary>
        ///     存储过程创建SQL 输入、输出参数化
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="entity"> 实体类 </param>
        IProcParam InitParam<TEntity>(TEntity entity) where TEntity : class, new();
        /// <summary>
        ///     将OutPut参数赋值到实体
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="entity"> 实体类 </param>
        void SetParamToEntity<TEntity>(TEntity entity) where TEntity : class, new();
    }
}