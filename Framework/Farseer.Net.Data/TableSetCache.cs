using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FS.Cache;
using FS.Data.Inteface;
using FS.Data.Internal;
using FS.Utils.Common.ExpressionVisitor;

namespace FS.Data
{
    /// <summary>
    ///     整表数据缓存Set，自动同步更新
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    public sealed class TableSetCache<TEntity> : IDbSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     数据缓存操作接口
        /// </summary>
        private readonly IDataCache<TEntity> _dataCache;

        /// <summary>
        ///     数据库操作
        /// </summary>
        private readonly TableSet<TEntity> _set;

        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 属性类型 </param>
        internal TableSetCache(DbContext context, PropertyInfo pInfo)
        {
            _set       = new TableSet<TEntity>(context: context, pInfo: pInfo);
            _dataCache = new DefaultDataCache<TEntity>(set: _set);
        }

        /// <summary>
        ///     当前缓存
        /// </summary>
        public IEnumerable<TEntity> Cache => _dataCache.Get();

        /// <summary>
        ///     更新缓存
        /// </summary>
        /// <param name="assign"> 赋值表达式树 </param>
        /// <param name="whereExp"> 条件 </param>
        private int UpdateCache(Expression assign, params Expression<Func<TEntity, bool>>[] whereExp)
        {
            // 更新本地缓存（立即执行，在事务中延迟会数据不同步）
            // 缓存库中的所有数据
            var lstCacheLibrary = Cache.ToList();

            // 筛选出要更新的数据列表
            var lstUpdate = whereExp.Aggregate(seed: lstCacheLibrary, func: (current, exp) => current.FindAll(match: new Predicate<TEntity>(exp.Compile())));

            // 筛选条件
            if (!lstUpdate.Any()) return 0;

            // 获取赋值表达式树中的每一项操作赋值符号树
            var lstLambda = new GetBinaryVisitor().Visit(exp: assign);
            foreach (var ex in lstLambda)
            {
                // 转换成Lambda表达式（用于下面的传入执行）
                // 获取实体类的表达式参数
                var entityParamter = new GetParamVisitor().Visit(exp: ex).FirstOrDefault();
                var lambda         = Expression.Lambda<Action<TEntity>>(body: ex, entityParamter).Compile();
                foreach (var updateEntity in lstUpdate) lambda(obj: updateEntity);
            }

            // 更新到接口操作的缓存保存中
            _dataCache.Update(lst: lstCacheLibrary);
            return lstCacheLibrary.Count;
        }

        #region 条件筛选器

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where"> 查询条件 </param>
        public TableSetCache<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            _set.Where(where: where);
            return this;
        }

        /// <summary> 自动生成o.ID == ID </summary>
        /// <param name="ID"> 值 </param>
        /// <param name="memberName"> 默认为主键ID属性（非数据库字段名称） </param>
        public TableSetCache<TEntity> Where<T>(T ID, string memberName = null)
        {
            _set.Where(value: ID, memberName: memberName);
            return this;
        }

        /// <summary> 自动生成lstIDs.Contains(o.ID) </summary>
        /// <param name="lstIDs"> </param>
        /// <param name="memberName"> 默认为主键ID属性（非数据库字段名称） </param>
        public TableSetCache<TEntity> Where<T>(List<T> lstIDs, string memberName = null)
        {
            _set.Where(lstvValues: lstIDs, memberName: memberName);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T"> 值类型 </typeparam>
        /// <param name="fieldName"> 字段选择器 </param>
        /// <param name="fieldValue"> 值 </param>
        public TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue)
        {
            _set.AddAssign(fieldName: fieldName, fieldValue: fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T"> 值类型 </typeparam>
        /// <param name="fieldName"> 字段选择器 </param>
        /// <param name="fieldValue"> 值 </param>
        public TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            _set.AddAssign(fieldName: fieldName, fieldValue: fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T"> 值类型 </typeparam>
        /// <param name="fieldName"> 字段选择器 </param>
        /// <param name="fieldValue"> 值 </param>
        public TableSetCache<TEntity> Append<T>(Expression<Func<TEntity, object>> fieldName, T fieldValue) where T : struct
        {
            _set.AddAssign(fieldName: fieldName, fieldValue: fieldValue);
            return this;
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
            var updateQueue = _set.Queue;
            _set.Update(entity: entity);

            // 由于Update时，有可能会产生条件（主键赋值被转成条件），故在Update之后，进行获取条件及赋值
            var assign   = updateQueue.ExpBuilder.ExpAssign;
            var whereExp = new GetBlockExpressionVisitor().Visit(exp: updateQueue.ExpBuilder.ExpWhere).OfType<Expression<Func<TEntity, bool>>>();

            // 更新本地缓存
            return UpdateCache(assign: assign, whereExp: whereExp.ToArray());
        }

        /// <summary>
        ///     修改（支持延迟加载）
        ///     如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"> </param>
        public async Task<int> UpdateAsync(TEntity entity)
        {
            var updateQueue = _set.Queue;
            await _set.UpdateAsync(entity: entity);

            // 由于Update时，有可能会产生条件（主键赋值被转成条件），故在Update之后，进行获取条件及赋值
            var assign   = updateQueue.ExpBuilder.ExpAssign;
            var whereExp = new GetBlockExpressionVisitor().Visit(exp: updateQueue.ExpBuilder.ExpWhere).OfType<Expression<Func<TEntity, bool>>>();

            // 更新本地缓存
            return UpdateCache(assign: assign, whereExp: whereExp.ToArray());
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity"> 实体类 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID == ID 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Update<T>(TEntity entity, T ID, string memberName = null) => Where(ID: ID, memberName: memberName).Update(entity: entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity"> 实体类 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID == ID 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> UpdateAsync<T>(TEntity entity, T ID, string memberName = null) => Where(ID: ID, memberName: memberName).UpdateAsync(entity: entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity"> 实体类 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Update<T>(TEntity entity, List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs: lstIDs, memberName: memberName).Update(entity: entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity"> 实体类 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> UpdateAsync<T>(TEntity entity, List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs: lstIDs, memberName: memberName).UpdateAsync(entity: entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity"> 实体类 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="where"> 查询条件 </param>
        public int Update(TEntity entity, Expression<Func<TEntity, bool>> where) => Where(where: where).Update(entity: entity);

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="entity"> 实体类 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="where"> 查询条件 </param>
        public Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> where) => Where(where: where).UpdateAsync(entity: entity);

        #endregion

        #region Insert

        /// <summary>
        ///     插入（支持延迟加载）会自动赋值标识字段
        /// </summary>
        /// <param name="entity"> </param>
        public int Insert(TEntity entity)
        {
            Check.NotNull(value: entity, parameterName: "插入操作时，参数不能为空！");

            // 更新本地缓存
            var lst = Cache.ToList();

            // 标识字段是否有值
            var indexHaveValue = _set.SetMap.PhysicsMap.DbGeneratedFields.Key != null && PropertyGetCacheManger.Cache(key: _set.SetMap.PhysicsMap.DbGeneratedFields.Key, instance: entity) != null;
            _set.Insert(entity: entity, isReturnLastId: !indexHaveValue);

            lst.Add(item: entity);
            _dataCache.Update(lst: lst);
            return 1;
        }

        /// <summary>
        ///     插入（支持延迟加载）会自动赋值标识字段
        /// </summary>
        /// <param name="entity"> </param>
        public async Task<int> InsertAsync(TEntity entity)
        {
            Check.NotNull(value: entity, parameterName: "插入操作时，参数不能为空！");

            // 更新本地缓存
            var lst = Cache.ToList();

            // 标识字段是否有值
            var indexHaveValue = _set.SetMap.PhysicsMap.DbGeneratedFields.Key != null && PropertyGetCacheManger.Cache(key: _set.SetMap.PhysicsMap.DbGeneratedFields.Key, instance: entity) != null;
            await _set.InsertAsync(entity: entity, isReturnLastId: !indexHaveValue);

            lst.Add(item: entity);
            _dataCache.Update(lst: lst);
            return 1;
        }

        /// <summary>
        ///     插入（不支持延迟加载）
        /// </summary>
        /// <param name="lst"> </param>
        public int Insert(List<TEntity> lst)
        {
            lst.ForEach(action: o => Insert(entity: o));
            return lst.Count;
        }

        /// <summary>
        ///     插入（不支持延迟加载）
        /// </summary>
        /// <param name="lst"> </param>
        public async Task<int> InsertAsync(List<TEntity> lst)
        {
            foreach (var entity in lst) await InsertAsync(entity: entity);
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
            var lst         = Cache.ToList();
            var deleteCount = lst.RemoveAll(match: new Predicate<TEntity>(exp.Compile()));
            _dataCache.Update(lst: lst);

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
            var lst         = Cache.ToList();
            var deleteCount = lst.RemoveAll(match: new Predicate<TEntity>(exp.Compile()));
            _dataCache.Update(lst: lst);

            return deleteCount;
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Delete<T>(T ID, string memberName = null) => Where(ID: ID, memberName: memberName).Delete();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> DeleteAsync<T>(T ID, string memberName = null) where T : struct => Where(ID: ID, memberName: memberName).DeleteAsync();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int Delete<T>(List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs: lstIDs, memberName: memberName).Delete();

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> DeleteAsync<T>(List<T> lstIDs, string memberName = null) where T : struct => Where(lstIDs: lstIDs, memberName: memberName).DeleteAsync();

        #endregion

        #region AddUp

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public int AddUp()
        {
            var assign = _set.Queue.ExpBuilder.ExpAssign;
            var exp    = (Expression<Func<TEntity, bool>>)_set.Queue.ExpBuilder.ExpWhere;
            _set.AddUp();

            // 更新本地缓存
            return UpdateCache(assign: assign, exp);
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public async Task<int> AddUpAsync()
        {
            var assign = _set.Queue.ExpBuilder.ExpAssign;
            var exp    = (Expression<Func<TEntity, bool>>)_set.Queue.ExpBuilder.ExpWhere;
            await _set.AddUpAsync();

            // 更新本地缓存
            return UpdateCache(assign: assign, exp);
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName"> 字段名称 </param>
        /// <param name="fieldValue"> 要+=的值 </param>
        public int AddUp<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct => Append(fieldName: fieldName, fieldValue: fieldValue).AddUp();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName"> 字段名称 </param>
        /// <param name="fieldValue"> 要+=的值 </param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct => Append(fieldName: fieldName, fieldValue: fieldValue).AddUpAsync();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName"> 字段名称 </param>
        /// <param name="fieldValue"> 要+=的值 </param>
        public int AddUp<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct => Append(fieldName: fieldName, fieldValue: fieldValue).AddUp();

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName"> 字段名称 </param>
        /// <param name="fieldValue"> 要+=的值 </param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct => Append(fieldName: fieldName, fieldValue: fieldValue).AddUpAsync();

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T"> 更新的值类型 </typeparam>
        /// <param name="select"> </param>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int AddUp<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct => Where(ID: ID.GetValueOrDefault(), memberName: memberName).AddUp(fieldName: select, fieldValue: fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T"> 更新的值类型 </typeparam>
        /// <param name="select"> </param>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> AddUpAsync<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct => Where(ID: ID.GetValueOrDefault(), memberName: memberName).AddUpAsync(fieldName: select, fieldValue: fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T"> 更新的值类型 </typeparam>
        /// <param name="select"> </param>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public int AddUp<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue, string memberName = null) where T : struct => Where(ID: ID, memberName: memberName).AddUp(fieldName: select, fieldValue: fieldValue);

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T"> 更新的值类型 </typeparam>
        /// <param name="select"> </param>
        /// <param name="fieldValue"> 要更新的值 </param>
        /// <param name="ID"> o => o.ID.Equals(ID) </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public Task<int> AddUpAsync<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue, string memberName = null) where T : struct => Where(ID: ID, memberName: memberName).AddUpAsync(fieldName: select, fieldValue: fieldValue);

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