using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Farseer.Net.Cache;
using Farseer.Net.Data.Infrastructure;
using Farseer.Net.Data.Internal;
using Farseer.Net.Utils.Common.ExpressionVisitor;

namespace Farseer.Net.Data
{
    /// <summary>
    ///     整表数据缓存Set，自动同步更新
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class TableSetCache<TEntity> : IDbSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        internal TableSetCache(DbContext context, PropertyInfo pInfo)
        {
            _set = new TableSet<TEntity>(context, pInfo);
            _dataCache = new DefaultDataCache<TEntity>(_set);
        }

        /// <summary>
        ///     数据库操作
        /// </summary>
        private readonly TableSet<TEntity> _set;
        /// <summary>
        ///     数据缓存操作接口
        /// </summary>
        private readonly IDataCache<TEntity> _dataCache;

        /// <summary>
        ///     当前缓存
        /// </summary>
        public IEnumerable<TEntity> Cache => _dataCache.Get();

        #region 条件筛选器
        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSetCache<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            _set.Where(where);
            return this;
        }

        /// <summary> 自动生成o.ID == ID </summary>
        /// <param name="ID">值</param>
        /// <param name="memberName">默认为主键ID属性（非数据库字段名称）</param>
        public TableSetCache<TEntity> Where<T>(T ID, string memberName = null)
        {
            _set.Where(ID, memberName);
            return this;
        }

        /// <summary> 自动生成lstIDs.Contains(o.ID) </summary>
        /// <param name="lstIDs"></param>
        /// <param name="memberName">默认为主键ID属性（非数据库字段名称）</param>
        public TableSetCache<TEntity> Where<T>(List<T> lstIDs, string memberName = null)
        {
            _set.Where(lstIDs, memberName);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue)
        {
            _set.AddAssign(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            _set.AddAssign(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, object>> fieldName, T fieldValue) where T : struct
        {
            _set.AddAssign(fieldName, fieldValue);
            return this;
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
            var updateQueue = _set.Queue;
            _set.Update(entity);

            // 由于Update时，有可能会产生条件（主键赋值被转成条件），故在Update之后，进行获取条件及赋值
            var assign = updateQueue.ExpBuilder.ExpAssign;
            var whereExp = new GetBlockExpressionVisitor().Visit(updateQueue.ExpBuilder.ExpWhere).OfType<Expression<Func<TEntity, bool>>>();

            // 更新本地缓存
            return UpdateCache(assign, whereExp.ToArray());
        }

        /// <summary>
        ///     修改（支持延迟加载）
        ///     如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"></param>
        public async Task<int> UpdateAsync(TEntity entity)
        {
            var updateQueue = _set.Queue;
            await _set.UpdateAsync(entity);

            // 由于Update时，有可能会产生条件（主键赋值被转成条件），故在Update之后，进行获取条件及赋值
            var assign = updateQueue.ExpBuilder.ExpAssign;
            var whereExp = new GetBlockExpressionVisitor().Visit(updateQueue.ExpBuilder.ExpWhere).OfType<Expression<Func<TEntity, bool>>>();

            // 更新本地缓存
            return UpdateCache(assign, whereExp.ToArray());
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Update<T>(TEntity entity, T ID, string memberName = null) => Where(ID, memberName).Update(entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> UpdateAsync<T>(TEntity entity, T ID, string memberName = null) => Where(ID, memberName).UpdateAsync(entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Update<T>(TEntity entity, List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs, memberName).Update(entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> UpdateAsync<T>(TEntity entity, List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs, memberName).UpdateAsync(entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="where">查询条件</param>
        public int Update(TEntity entity, Expression<Func<TEntity, bool>> where) => Where(where).Update(entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="where">查询条件</param>
        public Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> where) => Where(where).UpdateAsync(entity);
        #endregion

        #region Insert
        /// <summary>
        ///     插入（支持延迟加载）会自动赋值标识字段
        /// </summary>
        /// <param name="entity"></param>
        public int Insert(TEntity entity)
        {
            Check.NotNull(entity, "插入操作时，参数不能为空！");

            // 更新本地缓存
            var lst = Cache.ToList();

            // 标识字段是否有值
            var indexHaveValue = _set.SetMap.PhysicsMap.DbGeneratedFields.Key != null && PropertyGetCacheManger.Cache(_set.SetMap.PhysicsMap.DbGeneratedFields.Key, entity) != null;
            _set.Insert(entity, !indexHaveValue);

            lst.Add(entity);
            _dataCache.Update(lst);
            return 1;
        }

        /// <summary>
        ///     插入（支持延迟加载）会自动赋值标识字段
        /// </summary>
        /// <param name="entity"></param>
        public async Task<int> InsertAsync(TEntity entity)
        {
            Check.NotNull(entity, "插入操作时，参数不能为空！");

            // 更新本地缓存
            var lst = Cache.ToList();

            // 标识字段是否有值
            var indexHaveValue = _set.SetMap.PhysicsMap.DbGeneratedFields.Key != null && PropertyGetCacheManger.Cache(_set.SetMap.PhysicsMap.DbGeneratedFields.Key, entity) != null;
            await _set.InsertAsync(entity, !indexHaveValue);

            lst.Add(entity);
            _dataCache.Update(lst);
            return 1;
        }

        /// <summary>
        ///     插入（不支持延迟加载）
        /// </summary>
        /// <param name="lst"></param>
        public int Insert(List<TEntity> lst)
        {
            lst.ForEach(o => Insert(o));
            return lst.Count;
        }

        /// <summary>
        ///     插入（不支持延迟加载）
        /// </summary>
        /// <param name="lst"></param>
        public async Task<int> InsertAsync(List<TEntity> lst)
        {
            foreach (var entity in lst) { await InsertAsync(entity); }
            return lst.Count;
        }
        #endregion

        #region Delete
        /// <summary>
        ///     删除（支持延迟加载）
        /// </summary>
        public int Delete()
        {
            var exp = (Expression<Func<TEntity, bool>>)_set.Queue.ExpBuilder.ExpWhere;
            _set.Delete();

            // 更新本地缓存
            var lst = Cache.ToList();
            var deleteCount = lst.RemoveAll(new Predicate<TEntity>(exp.Compile()));
            _dataCache.Update(lst);

            return deleteCount;
        }

        /// <summary>
        ///     删除（支持延迟加载）
        /// </summary>
        public async Task<int> DeleteAsync()
        {
            var exp = (Expression<Func<TEntity, bool>>)_set.Queue.ExpBuilder.ExpWhere;
            await _set.DeleteAsync();

            // 更新本地缓存
            var lst = Cache.ToList();
            var deleteCount = lst.RemoveAll(new Predicate<TEntity>(exp.Compile()));
            _dataCache.Update(lst);

            return deleteCount;
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Delete<T>(T ID, string memberName = null) => Where(ID, memberName).Delete();

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
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Delete<T>(List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs, memberName).Delete();

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
            var assign = _set.Queue.ExpBuilder.ExpAssign;
            var exp = (Expression<Func<TEntity, bool>>)_set.Queue.ExpBuilder.ExpWhere;
            _set.AddUp();

            // 更新本地缓存
            return UpdateCache(assign, exp);
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public async Task<int> AddUpAsync()
        {
            var assign = _set.Queue.ExpBuilder.ExpAssign;
            var exp = (Expression<Func<TEntity, bool>>)_set.Queue.ExpBuilder.ExpWhere;
            await _set.AddUpAsync();

            // 更新本地缓存
            return UpdateCache(assign, exp);
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public int AddUp<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct => Append(fieldName, fieldValue).AddUp();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct => Append(fieldName, fieldValue).AddUpAsync();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public int AddUp<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct => Append(fieldName, fieldValue).AddUp();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct => Append(fieldName, fieldValue).AddUpAsync();

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int AddUp<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct => Where(ID.GetValueOrDefault(), memberName).AddUp(@select, fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> AddUpAsync<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct => Where(ID.GetValueOrDefault(), memberName).AddUpAsync(@select, fieldValue);

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
        public int AddUp<T>(T? ID, T fieldValue) where T : struct => AddUp(ID, null, fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <param name="fieldValue">要更新的值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public Task<int> AddUpAsync<T>(T? ID, T fieldValue) where T : struct => AddUpAsync(ID, null, fieldValue);
        #endregion

        /// <summary>
        ///     更新缓存
        /// </summary>
        /// <param name="assign">赋值表达式树</param>
        /// <param name="whereExp">条件</param>
        private int UpdateCache(Expression assign, params Expression<Func<TEntity, bool>>[] whereExp)
        {
            // 更新本地缓存（立即执行，在事务中延迟会数据不同步）
            // 缓存库中的所有数据
            var lstCacheLibrary = Cache.ToList();

            // 筛选出要更新的数据列表
            var lstUpdate = whereExp.Aggregate(lstCacheLibrary, (current, exp) => current.FindAll(new Predicate<TEntity>(exp.Compile())));

            // 筛选条件
            if (!lstUpdate.Any()) return 0;

            // 获取赋值表达式树中的每一项操作赋值符号树
            var lstLambda = new GetBinaryVisitor().Visit(assign);
            foreach (var ex in lstLambda)
            {
                // 转换成Lambda表达式（用于下面的传入执行）
                // 获取实体类的表达式参数
                var entityParamter = new GetParamVisitor().Visit(ex).FirstOrDefault();
                var lambda = Expression.Lambda<Action<TEntity>>(ex, entityParamter).Compile();
                foreach (var updateEntity in lstUpdate) lambda(updateEntity);
            }

            // 更新到接口操作的缓存保存中
            _dataCache.Update(lstCacheLibrary);
            return lstCacheLibrary.Count;
        }
    }
}