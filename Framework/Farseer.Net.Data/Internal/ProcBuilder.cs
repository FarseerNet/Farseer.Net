using System.Collections.Generic;
using System.Data.Common;
using Farseer.Net.Data.Infrastructure;
using Farseer.Net.Data.Map;

namespace Farseer.Net.Data.Internal
{
    /// <summary>
    ///     存储过程生成器
    /// </summary>
    internal class ProcBuilder : IProcParam
    {
        /// <summary>
        ///     存储过程生成器
        /// </summary>
        /// <param name="dbProvider">数据库驱动</param>
        /// <param name="setMap">实体类映射</param>
        /// <param name="name">存储过程名称</param>
        internal ProcBuilder(AbsDbProvider dbProvider, SetDataMap setMap, string name)
        {
            _dbProvider = dbProvider;
            _setMap = setMap;
            Name = name;
        }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        private readonly AbsDbProvider _dbProvider;

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        private readonly SetDataMap _setMap;

        /// <summary>
        ///     存储过程名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        public List<DbParameter> Param { get; private set; }

        /// <summary>
        ///     存储过程创建SQL 输入、输出参数化
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="entity">实体类</param>
        internal IProcParam InitParam<TEntity>(TEntity entity) where TEntity : class, new()
        {
            Param = _dbProvider.InitParam(_setMap.PhysicsMap, entity);
            return this;
        }

        /// <summary>
        ///     将OutPut参数赋值到实体
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="entity">实体类</param>
        internal void SetParamToEntity<TEntity>(TEntity entity) where TEntity : class, new()
        {
            _dbProvider.SetParamToEntity(_setMap.PhysicsMap, Param, entity);
        }
    }
}