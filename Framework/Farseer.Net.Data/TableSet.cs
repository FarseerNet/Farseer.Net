using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FS.Cache;
using FS.Data.Inteface;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Data
{
    /// <summary>
    ///     表操作
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    public sealed class TableSet<TEntity> : ReadDbSet<TableSet<TEntity>, TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 属性类型 </param>
        internal TableSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context: context, pInfo: pInfo);
        }

        #region 条件

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T"> 值类型 </typeparam>
        /// <param name="fieldName"> 字段选择器 </param>
        /// <param name="fieldValue"> 值 </param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue)
        {
            Queue.ExpBuilder.AddAssign(fieldName: fieldName, fieldValue: fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T"> 值类型 </typeparam>
        /// <param name="fieldName"> 字段选择器 </param>
        /// <param name="fieldValue"> 值 </param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            Queue.ExpBuilder.AddAssign(fieldName: fieldName, fieldValue: fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T"> 值类型 </typeparam>
        /// <param name="fieldName"> 字段选择器 </param>
        /// <param name="fieldValue"> 值 </param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, object>> fieldName, T fieldValue) where T : struct
        {
            Queue.ExpBuilder.AddAssign(fieldName: fieldName, fieldValue: fieldValue);
            return this;
        }

        #endregion

        #region Copy

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="acTEntity"> 对新职的赋值 </param>
        public void Copy(Action<TEntity> acTEntity = null)
        {
            var lst = ToList();
            foreach (var info in lst)
            {
                acTEntity?.Invoke(obj: info);
                Insert(entity: info);
            }
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="acTEntity"> 对新职的赋值 </param>
        public async Task CopyAsync(Action<TEntity> acTEntity = null)
        {
            var lst = ToList();
            foreach (var info in lst)
            {
                acTEntity?.Invoke(obj: info);
                await InsertAsync(entity: info);
            }
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act"> 对新职的赋值 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public void Copy<T>(T ID, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(value: ID, memberName: memberName);
            Copy(acTEntity: act);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act"> 对新职的赋值 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task CopyAsync<T>(T ID, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(value: ID, memberName: memberName);
            return CopyAsync(acTEntity: act);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act"> 对新职的赋值 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public void Copy<T>(IEnumerable<T> lstIDs, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(lstvValues: lstIDs, memberName: memberName);
            Copy(acTEntity: act);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act"> 对新职的赋值 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task CopyAsync<T>(IEnumerable<T> lstIDs, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(lstvValues: lstIDs, memberName: memberName);
            return CopyAsync(acTEntity: act);
        }

        #endregion

        #region Update

        /// <summary>
        ///     修改（支持延迟加载）
        ///     如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"> </param>
        public int Update(TEntity entity)
        {
            Check.IsTure(isTrue: entity == null && Queue.ExpBuilder.ExpAssign == null, parameterName: "更新操作时，参数不能为空！");

            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignUpdate(entity: entity);

            // 加入队列
            return QueueManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.Execute(callMethod: $"{typeof(TEntity).Name}.Update", sqlParam: queue.SqlBuilder.Update()), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     修改（支持延迟加载）
        ///     如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"> </param>
        public Task<int> UpdateAsync(TEntity entity)
        {
            Check.IsTure(isTrue: entity == null && Queue.ExpBuilder.ExpAssign == null, parameterName: "更新操作时，参数不能为空！");
            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignUpdate(entity: entity);

            // 加入队列
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ExecuteAsync(callMethod: $"{typeof(TEntity).Name}.UpdateAsync", sqlParam: queue.SqlBuilder.Update()), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info"> 实体类 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID == ID 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Update<T>(TEntity info, T ID, string memberName = null) => Where(value: ID, memberName: memberName).Update(entity: info);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info"> 实体类 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID == ID 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> UpdateAsync<T>(TEntity info, T ID, string memberName = null) => Where(value: ID, memberName: memberName).UpdateAsync(entity: info);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info"> 实体类 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Update<T>(TEntity info, IEnumerable<T> lstIDs, string memberName = null) => Where(lstvValues: lstIDs, memberName: memberName).Update(entity: info);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info"> 实体类 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> UpdateAsync<T>(TEntity info, IEnumerable<T> lstIDs, string memberName = null) where T : struct => Where(lstvValues: lstIDs, memberName: memberName).UpdateAsync(entity: info);

        #endregion

        #region Insert

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity"> </param>
        /// <param name="isReturnLastId"> 是否需要返回标识字段（如果设置的话） </param>
        public int Insert(TEntity entity, bool isReturnLastId = false)
        {
            Check.NotNull(value: entity, parameterName: "插入操作时，参数不能为空！");

            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignInsert(entity: entity);

            // 需要返回值时，则不允许延迟提交
            if (isReturnLastId && SetMap.PhysicsMap.DbGeneratedFields.Key != null)
            {
                // 赋值标识字段
                return QueueManger.Commit(map: SetMap, act: queue =>
                {
                    PropertySetCacheManger.Cache(key: SetMap.PhysicsMap.DbGeneratedFields.Key, instance: entity, value: ConvertHelper.ConvertType(sourceValue: Context.ExecuteSql.GetValue<object>(callMethod: $"{typeof(TEntity).Name}.InsertIdentity", sqlParam: queue.SqlBuilder.InsertIdentity()), returnType: SetMap.PhysicsMap.DbGeneratedFields.Key.PropertyType));
                    return 1;
                }, joinSoftDeleteCondition: false);
            }

            // 不返回标识字段
            return QueueManger.Commit(map: SetMap, act: queue =>
            {
                return Context.ExecuteSql.Execute(callMethod: $"{typeof(TEntity).Name}.Insert", sqlParam: queue.SqlBuilder.Insert());
            }, joinSoftDeleteCondition: false);
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity"> </param>
        /// <param name="isReturnLastId"> 是否需要返回标识字段（如果设置的话） </param>
        public Task<int> InsertAsync(TEntity entity, bool isReturnLastId = false)
        {
            Check.NotNull(value: entity, parameterName: "插入操作时，参数不能为空！");

            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignInsert(entity: entity);

            // 需要返回值时，则不允许延迟提交
            if (isReturnLastId && SetMap.PhysicsMap.DbGeneratedFields.Key != null)
            {
                // 赋值标识字段
                return QueueManger.CommitAsync(map: SetMap, act: async queue =>
                {
                    var sourceValue = await Context.ExecuteSql.GetValueAsync<object>(callMethod: $"{typeof(TEntity).Name}.InsertIdentityAsync", sqlParam: queue.SqlBuilder.InsertIdentity());
                    PropertySetCacheManger.Cache(key: SetMap.PhysicsMap.DbGeneratedFields.Key, instance: entity, value: ConvertHelper.ConvertType(sourceValue: sourceValue, returnType: SetMap.PhysicsMap.DbGeneratedFields.Key.PropertyType));
                    return 1;
                }, joinSoftDeleteCondition: false);
            }

            // 不返回标识字段
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ExecuteAsync(callMethod: $"{typeof(TEntity).Name}.InsertAsync", sqlParam: queue.SqlBuilder.Insert()), joinSoftDeleteCondition: false);
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity"> 实体类 </param>
        /// <param name="identity"> 返回标识字段（如果设置的话） </param>
        public int Insert<T>(TEntity entity, out T identity)
        {
            Check.NotNull(value: entity, parameterName: "插入操作时，entity参数不能为空！");
            if (SetMap.PhysicsMap.DbGeneratedFields.Key == null) throw new FarseerException(message: $"{entity.GetType().Name}未设置DbGenerated特性，无法获取自增ID");

            var result = Insert(entity: entity, isReturnLastId: true);

            // 获取标识字段
            identity = ConvertHelper.ConvertType(sourceValue: PropertyGetCacheManger.Cache(key: SetMap.PhysicsMap.DbGeneratedFields.Key, instance: entity), defValue: default(T));
            return result;
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="lst"> 实体类 </param>
        public int Insert(IEnumerable<TEntity> lst)
        {
            Check.NotNull(value: lst, parameterName: "插入操作时，lst参数不能为空！");

            if (Context.DbExecutor.DataType == eumDbType.SqlServer)
            {
                return QueueManger.Commit(map: SetMap, act: queue =>
                {
                    Context.DbExecutor.ExecuteSqlBulkCopy(tableName: SetMap.TableName, dt: lst.ToTable());
                    return lst.Count();
                }, joinSoftDeleteCondition: false);
            }

            return QueueManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.Execute(callMethod: $"{typeof(TEntity).Name}.InsertBatch", sqlParam: queue.SqlBuilder.InsertBatch(lst: lst)), joinSoftDeleteCondition: false);
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="lst"> 实体类 </param>
        public async Task<int> InsertAsync(IEnumerable<TEntity> lst)
        {
            Check.NotNull(value: lst, parameterName: "插入操作时，lst参数不能为空！");

            // 如果是SqlServer，则启用BulkCopy
            if (Context.DbExecutor.DataType == eumDbType.SqlServer)
            {
                return await QueueManger.CommitAsync(map: SetMap, act: async queue =>
                {
                    await Context.DbExecutor.ExecuteSqlBulkCopyAsync(tableName: SetMap.TableName, dt: lst.ToTable());
                    return lst.Count();
                }, joinSoftDeleteCondition: false);
            }
            if (Context.DbExecutor.DataType == eumDbType.ClickHouse) return await QueueManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ExecuteAsync(callMethod: $"{typeof(TEntity).Name}.InsertBatch", sqlParam: queue.SqlBuilder.InsertBatch(lst: lst)), joinSoftDeleteCondition: false);

            foreach (var entity in lst)
            {
                await InsertAsync(entity);
            }
            return lst.Count();
        }

        #endregion

        #region Delete

        /// <summary>
        ///     删除
        /// </summary>
        public int Delete()
        {
            if (SetMap.SortDelete != null)
            {
                Queue.ExpBuilder.AddAssign(fieldName: SetMap.SortDelete.AssignExpression);
                return Update(entity: null);
            }

            // 加入队列
            return QueueManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.Execute(callMethod: $"{typeof(TEntity).Name}.Delete", sqlParam: queue.SqlBuilder.Delete()), joinSoftDeleteCondition: false);
        }

        /// <summary>
        ///     删除
        /// </summary>
        public Task<int> DeleteAsync()
        {
            if (SetMap.SortDelete != null)
            {
                Queue.ExpBuilder.AddAssign(fieldName: SetMap.SortDelete.AssignExpression);
                return UpdateAsync(entity: null);
            }

            // 加入队列
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ExecuteAsync(callMethod: $"{typeof(TEntity).Name}.DeleteAsync", sqlParam: queue.SqlBuilder.Delete()), joinSoftDeleteCondition: false);
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Delete<T>(T ID, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).Delete();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> DeleteAsync<T>(T ID, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).DeleteAsync();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Delete<T>(T? ID, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).Delete();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> DeleteAsync<T>(T? ID, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).DeleteAsync();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Delete<T>(IEnumerable<T> lstIDs, string memberName = null) => Where(lstvValues: lstIDs, memberName: memberName).Delete();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> DeleteAsync<T>(IEnumerable<T> lstIDs, string memberName = null) where T : struct => Where(lstvValues: lstIDs, memberName: memberName).DeleteAsync();

        #endregion

        #region AddUp

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public int AddUp()
        {
            Check.NotNull(value: Queue.ExpBuilder.ExpAssign, parameterName: "+=字段操作时，必须先执行AddUp的另一个重载版本！");

            // 加入队列
            return QueueManger.Commit(map: SetMap, act: queue => Context.ExecuteSql.Execute(callMethod: $"{typeof(TEntity).Name}.AddUp", sqlParam: queue.SqlBuilder.AddUp()), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public Task<int> AddUpAsync()
        {
            Check.NotNull(value: Queue.ExpBuilder.ExpAssign, parameterName: "+=字段操作时，必须先执行AddUp的另一个重载版本！");

            // 加入队列
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.ExecuteSql.ExecuteAsync(callMethod: $"{typeof(TEntity).Name}.AddUpAsync", sqlParam: queue.SqlBuilder.AddUp()), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName"> 字段名称 </param>
        /// <param name="fieldValue"> 要+=的值 </param>
        public int AddUp<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct => AddAssign(fieldName: fieldName, fieldValue: fieldValue).AddUp();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName"> 字段名称 </param>
        /// <param name="fieldValue"> 要+=的值 </param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct => AddAssign(fieldName: fieldName, fieldValue: fieldValue).AddUpAsync();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName"> 字段名称 </param>
        /// <param name="fieldValue"> 要+=的值 </param>
        public int AddUp<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct => AddAssign(fieldName: fieldName, fieldValue: fieldValue).AddUp();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName"> 字段名称 </param>
        /// <param name="fieldValue"> 要+=的值 </param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct => AddAssign(fieldName: fieldName, fieldValue: fieldValue).AddUpAsync();

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T"> 更新的值类型 </typeparam>
        /// <param name="select"> </param>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int AddUp<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).AddUp(fieldName: select, fieldValue: fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T"> 更新的值类型 </typeparam>
        /// <param name="select"> </param>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> AddUpAsync<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).AddUpAsync(fieldName: select, fieldValue: fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T"> 更新的值类型 </typeparam>
        /// <param name="select"> </param>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int AddUp<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).AddUp(fieldName: select, fieldValue: fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T"> 更新的值类型 </typeparam>
        /// <param name="select"> </param>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> AddUpAsync<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).AddUpAsync(fieldName: select, fieldValue: fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        public int AddUp<T>(T? ID, T fieldValue) where T : struct => AddUp(ID: ID, select: null, fieldValue: fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        public Task<int> AddUpAsync<T>(T? ID, T fieldValue) where T : struct => AddUpAsync(ID: ID, select: null, fieldValue: fieldValue);

        #endregion
    }
}