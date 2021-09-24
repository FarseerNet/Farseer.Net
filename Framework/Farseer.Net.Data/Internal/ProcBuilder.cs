using System.Collections.Generic;
using System.Data.Common;
using FS.Data.Client;
using FS.Data.Infrastructure;
using FS.Data.Map;

namespace FS.Data.Internal
{
    /// <summary>
    ///     存储过程生成器
    /// </summary>
    internal class ProcBuilder : IProcParam
    {
        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        private readonly AbsDbProvider _dbProvider;

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        private readonly SetDataMap _setMap;

        /// <summary>
        ///     存储过程生成器
        /// </summary>
        /// <param name="dbProvider"> 数据库驱动 </param>
        /// <param name="setMap"> 实体类映射 </param>
        internal ProcBuilder(AbsDbProvider dbProvider, SetDataMap setMap)
        {
            _dbProvider = dbProvider;
            _setMap     = setMap;
            ProcName    = setMap.TableName;
            DbName      = setMap.DbName;
        }

        /// <summary>
        ///     数据库名称
        /// </summary>
        public string DbName { get; }

        /// <summary>
        ///     存储过程名称
        /// </summary>
        public string ProcName { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        public List<DbParameter> Param { get; private set; }

        /// <summary>
        ///     存储过程创建SQL 输入、输出参数化
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="entity"> 实体类 </param>
        internal IProcParam InitParam<TEntity>(TEntity entity) where TEntity : class, new()
        {
            Param = _dbProvider.InitParam(map: _setMap.PhysicsMap, entity: entity);
            return this;
        }

        /// <summary>
        ///     将OutPut参数赋值到实体
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="entity"> 实体类 </param>
        internal void SetParamToEntity<TEntity>(TEntity entity) where TEntity : class, new()
        {
            _dbProvider.SetParamToEntity(map: _setMap.PhysicsMap, lstParam: Param, entity: entity);
        }
    }
}