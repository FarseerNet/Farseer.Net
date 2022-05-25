using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.Data.Inteface;

namespace FS.Data
{
    public class SqlSet<TEntity> : SqlSet, IDbSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 属性类型 </param>
        internal SqlSet(DbContext context, PropertyInfo pInfo) : base(context: context, pInfo: pInfo)
        {
        }

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity"> 将entity中的InParam参数转化为Sql参数 </param>
        public TEntity ToEntity(TEntity entity = null) => Context.ManualSql.ToEntity<TEntity>(sql: Map.Sql, parameters: Context.DbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToArray());

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity"> 将entity中的InParam参数转化为Sql参数 </param>
        public Task<TEntity> ToEntityAsync(TEntity entity = null) => Context.ManualSql.ToEntityAsync<TEntity>(sql: Map.Sql, parameters: Context.DbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToArray());

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 将entity中的InParam参数转化为Sql参数 </param>
        public List<TEntity> ToList(TEntity entity = null) => Context.ManualSql.ToList<TEntity>(sql: Map.Sql, parameters: Context.DbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToArray());

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 将entity中的InParam参数转化为Sql参数 </param>
        public Task<List<TEntity>> ToListAsync(TEntity entity = null) => Context.ManualSql.ToListAsync<TEntity>(sql: Map.Sql, parameters: Context.DbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToArray());

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity"> 将entity中的InParam参数转化为Sql参数 </param>
        /// <param name="t"> 失败时返回的值 </param>
        public T GetValue<T>(TEntity entity = null, T t = default) => Context.ManualSql.GetValue(sql: Map.Sql, t: t, parameters: Context.DbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToArray());

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity"> 将entity中的InParam参数转化为Sql参数 </param>
        /// <param name="t"> 失败时返回的值 </param>
        public Task<T> GetValueAsync<T>(TEntity entity = null, T t = default) => Context.ManualSql.GetValueAsync(sql: Map.Sql, t: t, parameters: Context.DbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToArray());

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity"> 将entity中的InParam参数转化为Sql参数 </param>
        public int Execute(TEntity entity = null) => Context.ManualSql.Execute(sql: Map.Sql, parameters: Context.DbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToArray());

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity"> 将entity中的InParam参数转化为Sql参数 </param>
        public Task<int> ExecuteAsync(TEntity entity = null) => Context.ManualSql.ExecuteAsync(sql: Map.Sql, parameters: Context.DbProvider.DbParam.InitParam(map: SetMap.PhysicsMap, entity: entity).ToArray());
    }
}