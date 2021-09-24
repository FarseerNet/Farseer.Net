using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using FS.Data.Infrastructure;

namespace FS.Data
{
    /// <summary>
    ///     存储过程操作
    /// </summary>
    /// <typeparam name="TEntity"> 实体 </typeparam>
    public sealed class ProcSet<TEntity> : AbsDbSet, IDbSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 属性类型 </param>
        internal ProcSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context: context, pInfo: pInfo);
        }

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        /// <param name="t"> 失败时返回的值 </param>
        public T GetValue<T>(TEntity entity = null, T t = default) => QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.GetValue(callMethod: $"{typeof(TEntity).Name}.GetValue", procBuilder: Queue.ProcBuilder, entity: entity, defValue: t), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        /// <param name="t"> 失败时返回的值 </param>
        public Task<T> GetValueAsync<T>(TEntity entity = null, T t = default) => QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.GetValueAsync(callMethod: $"{typeof(TEntity).Name}.GetValueAsync", procBuilder: Queue.ProcBuilder, entity: entity, defValue: t), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public TEntity ToEntity(TEntity entity = null) => QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.ToEntity(callMethod: $"{typeof(TEntity).Name}.ToEntity", procBuilder: Queue.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public Task<TEntity> ToEntityAsync(TEntity entity = null) => QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ToEntityAsync(callMethod: $"{typeof(TEntity).Name}.ToEntityAsync", procBuilder: Queue.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public List<TEntity> ToList(TEntity entity = null) => QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.ToList(callMethod: $"{typeof(TEntity).Name}.ToList", procBuilder: Queue.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public Task<List<TEntity>> ToListAsync(TEntity entity = null) => QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ToListAsync(callMethod: $"{typeof(TEntity).Name}.ToListAsync", procBuilder: Queue.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public DataTable ToTable(TEntity entity = null) => QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.ToTable(callMethod: $"{typeof(TEntity).Name}.ToTable", procBuilder: Queue.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public Task<DataTable> ToTableAsync(TEntity entity = null) => QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ToTableAsync(callMethod: $"{typeof(TEntity).Name}.ToTableAsync", procBuilder: Queue.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public int Execute(TEntity entity = null)
        {
            // 加入委托
            //var isExitsOutParam = SetMap.PhysicsMap.MapList.Any(o => o.Value.Field.IsOutParam);
            return QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.Execute(callMethod: $"{typeof(TEntity).Name}.Execute", procBuilder: Queue.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);
        }

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public Task<int> ExecuteAsync(TEntity entity = null)
        {
            // 加入委托
            //var isExitsOutParam = SetMap.PhysicsMap.MapList.Any(o => o.Value.Field.IsOutParam);
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ExecuteAsync(callMethod: $"{typeof(TEntity).Name}.ExecuteAsync", procBuilder: Queue.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);
        }
    }
}