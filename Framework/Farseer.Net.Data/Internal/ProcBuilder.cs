using System;
using System.Data.Common;
using Collections.Pooled;
using FS.Data.Client;
using FS.Data.Inteface;
using FS.Data.Map;

namespace FS.Data.Internal
{
    /// <summary>
    ///     存储过程生成器
    /// </summary>
    internal class ProcBuilder : IProcParam, IDisposable
    {
        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        private readonly AbsDbProvider _dbProvider;

        /// <summary>
        ///     存储过程生成器
        /// </summary>
        /// <param name="dbProvider"> 数据库驱动 </param>
        /// <param name="setMap"> 实体类映射 </param>
        internal ProcBuilder(AbsDbProvider dbProvider, SetDataMap setMap)
        {
            _dbProvider = dbProvider;
            SetMap      = setMap;
            ProcName    = setMap.TableName;
        }

        /// <summary>
        /// 实体类结构映射
        /// </summary>
        public SetDataMap SetMap { get; }

        /// <summary>
        ///     存储过程名称
        /// </summary>
        public string ProcName { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        public PooledList<DbParameter> Param { get; private set; }

        /// <summary>
        ///     存储过程创建SQL 输入、输出参数化
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="entity"> 实体类 </param>
        internal IProcParam InitParam<TEntity>(TEntity entity) where TEntity : class, new()
        {
            Param = _dbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToPooledList();
            return this;
        }

        /// <summary>
        ///     将OutPut参数赋值到实体
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="entity"> 实体类 </param>
        internal void SetParamToEntity<TEntity>(TEntity entity) where TEntity : class, new()
        {
            _dbProvider.DbParam.SetParamToEntity(map: SetMap.PhysicsMap, lstParam: Param, entity: entity);
        }

        public void Dispose()
        {
            Param.Dispose();
        }
    }
}