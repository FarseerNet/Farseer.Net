﻿using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Data.Abstract;

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
        public T GetValue<T>(TEntity entity = null, T t = default) => QueryManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.GetValue(callMethod: $"{typeof(TEntity).Name}.GetValue", procBuilder: Query.ProcBuilder, entity: entity, defValue: t), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        /// <param name="t"> 失败时返回的值 </param>
        public Task<T> GetValueAsync<T>(TEntity entity = null, T t = default) => QueryManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.GetValueAsync(callMethod: $"{typeof(TEntity).Name}.GetValueAsync", procBuilder: Query.ProcBuilder, entity: entity, defValue: t), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public TEntity ToEntity(TEntity entity = null) => QueryManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.ToEntity(callMethod: $"{typeof(TEntity).Name}.ToEntity", procBuilder: Query.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public Task<TEntity> ToEntityAsync(TEntity entity = null) => QueryManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ToEntityAsync(callMethod: $"{typeof(TEntity).Name}.ToEntityAsync", procBuilder: Query.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public PooledList<TEntity> ToList(TEntity entity = null) => QueryManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.ToList(callMethod: $"{typeof(TEntity).Name}.ToList", procBuilder: Query.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public Task<PooledList<TEntity>> ToListAsync(TEntity entity = null) => QueryManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ToListAsync(callMethod: $"{typeof(TEntity).Name}.ToListAsync", procBuilder: Query.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public DataTable ToTable(TEntity entity = null) => QueryManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.ToTable(callMethod: $"{typeof(TEntity).Name}.ToTable", procBuilder: Query.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public Task<DataTable> ToTableAsync(TEntity entity = null) => QueryManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ToTableAsync(callMethod: $"{typeof(TEntity).Name}.ToTableAsync", procBuilder: Query.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public int Execute(TEntity entity = null)
        {
            // 加入委托
            //var isExitsOutParam = SetMap.PhysicsMap.MapList.Any(o => o.Value.Field.IsOutParam);
            return QueryManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.Execute(callMethod: $"{typeof(TEntity).Name}.Execute", procBuilder: Query.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);
        }

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="entity"> 传入被设置好参数赋值的实体 </param>
        public Task<int> ExecuteAsync(TEntity entity = null)
        {
            // 加入委托
            //var isExitsOutParam = SetMap.PhysicsMap.MapList.Any(o => o.Value.Field.IsOutParam);
            return QueryManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ExecuteAsync(callMethod: $"{typeof(TEntity).Name}.ExecuteAsync", procBuilder: Query.ProcBuilder, entity: entity), joinSoftDeleteCondition: false);
        }
    }
}