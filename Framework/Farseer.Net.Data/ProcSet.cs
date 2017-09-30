using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Farseer.Net.Data.Infrastructure;
using Farseer.Net.Data.Internal;

namespace Farseer.Net.Data
{
    /// <summary>
    ///     存储过程操作
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public sealed class ProcSet<TEntity> : AbsDbSet, IDbSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        internal ProcSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context, pInfo);
        }

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        /// <param name="t">失败时返回的值</param>
        public T GetValue<T>(TEntity entity = null, T t = default(T)) => QueueManger.Commit(SetMap, (queue) => Context.Executeor.GetValue(Queue.ProcBuilder, entity, t), false);

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        /// <param name="t">失败时返回的值</param>
        public Task<T> GetValueAsync<T>(TEntity entity = null, T t = default(T)) => QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.GetValueAsync(Queue.ProcBuilder, entity, t), false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        public TEntity ToEntity(TEntity entity = null) => QueueManger.Commit(SetMap, (queue) => Context.Executeor.ToEntity(Queue.ProcBuilder, entity), false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        public Task<TEntity> ToEntityAsync(TEntity entity = null) => QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.ToEntityAsync(Queue.ProcBuilder, entity), false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        public List<TEntity> ToList(TEntity entity = null) => QueueManger.Commit(SetMap, (queue) => Context.Executeor.ToList<TEntity>(Queue.ProcBuilder, entity), false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        public Task<List<TEntity>> ToListAsync(TEntity entity = null) => QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.ToListAsync<TEntity>(Queue.ProcBuilder, entity), false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        public DataTable ToTable(TEntity entity = null) => QueueManger.Commit(SetMap, (queue) => Context.Executeor.ToTable(Queue.ProcBuilder, entity), false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        public Task<DataTable> ToTableAsync(TEntity entity = null) => QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.ToTableAsync(Queue.ProcBuilder, entity), false);

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        public int Execute(TEntity entity = null)
        {
            // 加入委托
            //var isExitsOutParam = SetMap.PhysicsMap.MapList.Any(o => o.Value.Field.IsOutParam);
            return QueueManger.Commit(SetMap, (queue) => Context.Executeor.Execute(Queue.ProcBuilder, entity), false);
        }

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity">传入被设置好参数赋值的实体</param>
        public Task<int> ExecuteAsync(TEntity entity = null)
        {
            // 加入委托
            //var isExitsOutParam = SetMap.PhysicsMap.MapList.Any(o => o.Value.Field.IsOutParam);
            return QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.ExecuteAsync(Queue.ProcBuilder, entity), false);
        }
    }
}