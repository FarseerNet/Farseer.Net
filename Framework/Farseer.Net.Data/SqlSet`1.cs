using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FS.Data.Infrastructure;

namespace FS.Data
{
    public class SqlSet<TEntity> : SqlSet, IDbSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        internal SqlSet(DbContext context, PropertyInfo pInfo) : base(context, pInfo) { }

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity">将entity中的InParam参数转化为Sql参数</param>
        public TEntity ToEntity(TEntity entity = null) => Context.ManualSql.ToEntity<TEntity>(Map.Sql, Context.DbProvider.InitParam(SetMap.PhysicsMap, entity).ToArray());

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity">将entity中的InParam参数转化为Sql参数</param>
        public Task<TEntity> ToEntityAsync(TEntity entity = null) => Context.ManualSql.ToEntityAsync<TEntity>(Map.Sql, Context.DbProvider.InitParam(SetMap.PhysicsMap, entity).ToArray());

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity">将entity中的InParam参数转化为Sql参数</param>
        public List<TEntity> ToList(TEntity entity = null) => Context.ManualSql.ToList<TEntity>(Map.Sql, Context.DbProvider.InitParam(SetMap.PhysicsMap, entity).ToArray());

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity">将entity中的InParam参数转化为Sql参数</param>
        public Task<List<TEntity>> ToListAsync(TEntity entity = null) => Context.ManualSql.ToListAsync<TEntity>(Map.Sql, Context.DbProvider.InitParam(SetMap.PhysicsMap, entity).ToArray());

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity">将entity中的InParam参数转化为Sql参数</param>
        /// <param name="t">失败时返回的值</param>
        public T GetValue<T>(TEntity entity = null, T t = default(T)) => Context.ManualSql.GetValue(Map.Sql, t, Context.DbProvider.InitParam(SetMap.PhysicsMap, entity).ToArray());

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity">将entity中的InParam参数转化为Sql参数</param>
        /// <param name="t">失败时返回的值</param>
        public Task<T> GetValueAsync<T>(TEntity entity = null, T t = default(T)) => Context.ManualSql.GetValueAsync(Map.Sql, t, Context.DbProvider.InitParam(SetMap.PhysicsMap, entity).ToArray());

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity">将entity中的InParam参数转化为Sql参数</param>
        public int Execute(TEntity entity = null) => Context.ManualSql.Execute(Map.Sql, Context.DbProvider.InitParam(SetMap.PhysicsMap, entity).ToArray());

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity">将entity中的InParam参数转化为Sql参数</param>
        public Task<int> ExecuteAsync(TEntity entity = null) => Context.ManualSql.ExecuteAsync(Map.Sql, Context.DbProvider.InitParam(SetMap.PhysicsMap, entity).ToArray());
    }
}