using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FS.Cache;
using FS.Data.Infrastructure;
using FS.Extends;
using FS.Utils.Common;
using Newtonsoft.Json;

namespace FS.Data
{
    /// <summary>
    ///     表操作
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class TableSet<TEntity> : ReadDbSet<TableSet<TEntity>, TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        internal TableSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context, pInfo);
        }

        #region 条件

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue)
        {
            Queue.ExpBuilder.AddAssign(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            Queue.ExpBuilder.AddAssign(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, object>> fieldName, T fieldValue) where T : struct
        {
            Queue.ExpBuilder.AddAssign(fieldName, fieldValue);
            return this;
        }

        #endregion

        #region Copy

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="acTEntity">对新职的赋值</param>
        public void Copy(Action<TEntity> acTEntity = null)
        {
            var lst = ToList();
            foreach (var info in lst)
            {
                acTEntity?.Invoke(info);
                Insert(info);
            }
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="acTEntity">对新职的赋值</param>
        public async Task CopyAsync(Action<TEntity> acTEntity = null)
        {
            var lst = ToList();
            foreach (var info in lst)
            {
                acTEntity?.Invoke(info);
                await InsertAsync(info);
            }
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public void Copy<T>(T ID, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(ID, memberName);
            Copy(act);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task CopyAsync<T>(T ID, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(ID, memberName);
            return CopyAsync(act);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public void Copy<T>(List<T> lstIDs, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(lstIDs, memberName);
            Copy(act);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task CopyAsync<T>(List<T> lstIDs, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(lstIDs, memberName);
            return CopyAsync(act);
        }

        #endregion

        #region Update

        /// <summary>
        ///     修改（支持延迟加载）
        ///     如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"></param>
        public int Update(TEntity entity)
        {
            Check.IsTure(entity == null && Queue.ExpBuilder.ExpAssign == null, "更新操作时，参数不能为空！");

            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignUpdate(entity);

            // 加入队列
            return QueueManger.Commit(SetMap, (queue) => Context.Executeor.Execute($"{typeof(TEntity).Name}.Update", queue.SqlBuilder.Update()), true);
        }

        /// <summary>
        ///     修改（支持延迟加载）
        ///     如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"></param>
        public Task<int> UpdateAsync(TEntity entity)
        {
            Check.IsTure(entity == null && Queue.ExpBuilder.ExpAssign == null, "更新操作时，参数不能为空！");
            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignUpdate(entity);

            // 加入队列
            return QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.ExecuteAsync($"{typeof(TEntity).Name}.UpdateAsync", queue.SqlBuilder.Update()), true);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Update<T>(TEntity info, T ID, string memberName = null) => Where(ID, memberName).Update(info);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> UpdateAsync<T>(TEntity info, T ID, string memberName = null) => Where(ID, memberName).UpdateAsync(info);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Update<T>(TEntity info, List<T> lstIDs, string memberName = null) => Where(lstIDs, memberName).Update(info);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> UpdateAsync<T>(TEntity info, List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs, memberName).UpdateAsync(info);

        #endregion

        #region Insert

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isReturnLastId">是否需要返回标识字段（如果设置的话）</param>
        public int Insert(TEntity entity, bool isReturnLastId = false)
        {
            Check.NotNull(entity, "插入操作时，参数不能为空！");

            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignInsert(entity);

            // 需要返回值时，则不允许延迟提交
            if (isReturnLastId && SetMap.PhysicsMap.DbGeneratedFields.Key != null)
            {
                // 赋值标识字段
                return QueueManger.Commit(SetMap, (queue) =>
                {
                    PropertySetCacheManger.Cache(SetMap.PhysicsMap.DbGeneratedFields.Key, entity, ConvertHelper.ConvertType(Context.Executeor.GetValue<object>($"{typeof(TEntity).Name}.InsertIdentity", queue.SqlBuilder.InsertIdentity()), SetMap.PhysicsMap.DbGeneratedFields.Key.PropertyType));
                    return 1;
                }, false);
            }

            // 不返回标识字段
            return QueueManger.Commit(SetMap, (queue) => Context.Executeor.Execute($"{typeof(TEntity).Name}.Insert", queue.SqlBuilder.Insert()), false);
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isReturnLastId">是否需要返回标识字段（如果设置的话）</param>
        public Task<int> InsertAsync(TEntity entity, bool isReturnLastId = false)
        {
            Check.NotNull(entity, "插入操作时，参数不能为空！");

            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignInsert(entity);

            // 需要返回值时，则不允许延迟提交
            if (isReturnLastId && SetMap.PhysicsMap.DbGeneratedFields.Key != null)
            {
                // 赋值标识字段
                return QueueManger.CommitAsync(SetMap, async (queue) =>
                {
                    var sourceValue = await Context.Executeor.GetValueAsync<object>($"{typeof(TEntity).Name}.InsertIdentityAsync", queue.SqlBuilder.InsertIdentity());
                    PropertySetCacheManger.Cache(SetMap.PhysicsMap.DbGeneratedFields.Key, entity, ConvertHelper.ConvertType(sourceValue, SetMap.PhysicsMap.DbGeneratedFields.Key.PropertyType));
                    return 1;
                }, false);
            }

            // 不返回标识字段
            return QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.ExecuteAsync($"{typeof(TEntity).Name}.InsertAsync", queue.SqlBuilder.Insert()), false);
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="identity">返回标识字段（如果设置的话）</param>
        public int Insert<T>(TEntity entity, out T identity)
        {
            Check.NotNull(entity, "插入操作时，entity参数不能为空！");
            if (SetMap.PhysicsMap.DbGeneratedFields.Key == null)
            {
                throw new FarseerException($"{entity.GetType().Name}未设置DbGenerated特性，无法获取自增ID");
            }

            var result = Insert(entity, true);

            // 获取标识字段
            identity = ConvertHelper.ConvertType(PropertyGetCacheManger.Cache(SetMap.PhysicsMap.DbGeneratedFields.Key, entity), default(T));
            return result;
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="lst">实体类</param>
        public int Insert(List<TEntity> lst)
        {
            Check.NotNull(lst, "插入操作时，lst参数不能为空！");

            if (Context.Executeor.DataBase.DataType == eumDbType.SqlServer)
            {
                return QueueManger.Commit(SetMap, (queue) =>
                {
                    Context.Executeor.DataBase.ExecuteSqlBulkCopy(SetMap.TableName, lst.ToTable());
                    return lst.Count;
                }, false);
            }

            return QueueManger.Commit(SetMap, queue => Context.Executeor.Execute($"{typeof(TEntity).Name}.InsertBatch", queue.SqlBuilder.InsertBatch(lst)), false);
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="lst">实体类</param>
        public async Task<int> InsertAsync(List<TEntity> lst)
        {
            Check.NotNull(lst, "插入操作时，lst参数不能为空！");

            // 如果是SqlServer，则启用BulkCopy
            if (Context.Executeor.DataBase.DataType == eumDbType.SqlServer)
            {
                return await QueueManger.CommitAsync(SetMap, async (queue) =>
                {
                    await Context.Executeor.DataBase.ExecuteSqlBulkCopyAsync(SetMap.TableName, lst.ToTable());
                    return lst.Count;
                }, false);
            }

            return await QueueManger.CommitAsync(SetMap, queue => Context.Executeor.ExecuteAsync($"{typeof(TEntity).Name}.InsertBatch", queue.SqlBuilder.InsertBatch(lst)), false);
            
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
                Queue.ExpBuilder.AddAssign(SetMap.SortDelete.AssignExpression);
                return Update(null);
            }

            // 加入队列
            return QueueManger.Commit(SetMap, (queue) => Context.Executeor.Execute($"{typeof(TEntity).Name}.Delete", queue.SqlBuilder.Delete()), false);
        }

        /// <summary>
        ///     删除
        /// </summary>
        public Task<int> DeleteAsync()
        {
            if (SetMap.SortDelete != null)
            {
                Queue.ExpBuilder.AddAssign(SetMap.SortDelete.AssignExpression);
                return UpdateAsync(null);
            }

            // 加入队列
            return QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.ExecuteAsync($"{typeof(TEntity).Name}.DeleteAsync", queue.SqlBuilder.Delete()), false);
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Delete<T>(T ID, string memberName = null) where T : struct => Where(ID, memberName).Delete();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> DeleteAsync<T>(T ID, string memberName = null) where T : struct => Where(ID, memberName).DeleteAsync();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Delete<T>(T? ID, string memberName = null) where T : struct => Where(ID, memberName).Delete();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> DeleteAsync<T>(T? ID, string memberName = null) where T : struct => Where(ID, memberName).DeleteAsync();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Delete<T>(List<T> lstIDs, string memberName = null) => Where(lstIDs, memberName).Delete();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> DeleteAsync<T>(List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs, memberName).DeleteAsync();

        #endregion

        #region AddUp

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public int AddUp()
        {
            Check.NotNull(Queue.ExpBuilder.ExpAssign, "+=字段操作时，必须先执行AddUp的另一个重载版本！");

            // 加入队列
            return QueueManger.Commit(SetMap, (queue) => Context.Executeor.Execute($"{typeof(TEntity).Name}.AddUp", queue.SqlBuilder.AddUp()), true);
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public Task<int> AddUpAsync()
        {
            Check.NotNull(Queue.ExpBuilder.ExpAssign, "+=字段操作时，必须先执行AddUp的另一个重载版本！");

            // 加入队列
            return QueueManger.CommitAsync(SetMap, (queue) => Context.Executeor.ExecuteAsync($"{typeof(TEntity).Name}.AddUpAsync", queue.SqlBuilder.AddUp()), true);
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public int AddUp<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct => AddAssign(fieldName, fieldValue).AddUp();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct => AddAssign(fieldName, fieldValue).AddUpAsync();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public int AddUp<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct => AddAssign(fieldName, fieldValue).AddUp();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct => AddAssign(fieldName, fieldValue).AddUpAsync();

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int AddUp<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct => Where(ID, memberName).AddUp(@select, fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> AddUpAsync<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct => Where(ID, memberName).AddUpAsync(@select, fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int AddUp<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue, string memberName = null) where T : struct => Where(ID, memberName).AddUp(@select, fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> AddUpAsync<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue, string memberName = null) where T : struct => Where(ID, memberName).AddUpAsync(@select, fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <param name="fieldValue">要更新的值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public int AddUp<T>(T? ID, T fieldValue) where T : struct => AddUp<T>(ID, null, fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <param name="fieldValue">要更新的值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public Task<int> AddUpAsync<T>(T? ID, T fieldValue) where T : struct => AddUpAsync<T>(ID, null, fieldValue);

        #endregion
    }
}