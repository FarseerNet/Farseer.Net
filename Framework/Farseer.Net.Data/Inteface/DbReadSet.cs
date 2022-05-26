using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FS.Core;
using FS.Core.Abstract.Data;
using FS.Data.Features;
using FS.Utils.Common;

namespace FS.Data.Inteface
{
    /// <summary>
    ///     通用查询Set（表、视图）
    /// </summary>
    /// <typeparam name="TSet"> </typeparam>
    /// <typeparam name="TEntity"> </typeparam>
    public abstract class ReadDbSet<TSet, TEntity> : AbsDbSet, IDbSet<TEntity> where TSet : ReadDbSet<TSet, TEntity> where TEntity : class, new()
    {
        public TSet SetName(string tableName)
        {
            SetMap.SetName(tableName: tableName);
            return (TSet)this;
        }

        public TSet SetName(string dbName, string tableName)
        {
            SetMap.SetName(dbName, tableName);
            return (TSet)this;
        }

        public TSet SetName(string dbName, string tableName, EumTableEnginesType eumTableEnginesType)
        {
            SetMap.SetName(dbName, tableName, eumTableEnginesType);
            return (TSet)this;
        }

        #region 条件筛选器

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        public virtual TSet Select<T>(Expression<Func<TEntity, T>> select)
        {
            Queue.ExpBuilder.AddSelect(select: select);
            return (TSet)this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where"> 查询条件 </param>
        public virtual TSet Where(Expression<Func<TEntity, bool>> where)
        {
            Queue.ExpBuilder.AddWhere(where: where);
            return (TSet)this;
        }

        /// <summary>
        ///     查询条件添加或者条件
        /// </summary>
        /// <param name="where"> 查询条件 </param>
        public virtual TSet WhereOr(Expression<Func<TEntity, bool>> where)
        {
            Queue.ExpBuilder.AddWhereOr(where: where);
            return (TSet)this;
        }

        /// <summary> 自动生成o.ID == ID </summary>
        /// <param name="value"> 值 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual TSet Where<T>(T value, string memberName = null)
        {
            if (string.IsNullOrWhiteSpace(value: memberName))
            {
                Check.IsTure(isTrue: SetMap.PhysicsMap.PrimaryFields.Count > 1, parameterName: "主键设置了多个字段，无法猜到使用哪个做过条件字段，请手动设置memberName参数");
                memberName = SetMap.PhysicsMap.PrimaryFields.Count > 0 ? SetMap.PhysicsMap.PrimaryFields.First().Value.Name : "ID";
            }

            Where(where: ExpressionHelper.CreateBinaryExpression<TEntity>(val: value, memberName: memberName));
            return (TSet)this;
        }

        /// <summary> 自动生成lstIDs.Contains(o.ID) </summary>
        /// <param name="lstvValues"> </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual TSet Where<T>(List<T> lstvValues, string memberName = null)
        {
            if (string.IsNullOrWhiteSpace(value: memberName))
            {
                Check.IsTure(isTrue: SetMap.PhysicsMap.PrimaryFields.Count > 1, parameterName: "主键设置了多个字段，无法猜到使用哪个做过条件字段，请手动设置memberName参数");
                memberName = SetMap.PhysicsMap.PrimaryFields.Count > 0 ? SetMap.PhysicsMap.PrimaryFields.First().Value.Name : "ID";
            }

            Where(where: ExpressionHelper.CreateContainsBinaryExpression<TEntity>(val: lstvValues, memberName: memberName));
            return (TSet)this;
        }

        /// <summary>
        ///     倒序查询
        /// </summary>
        /// <typeparam name="TKey"> 实体类属性类型 </typeparam>
        /// <param name="desc"> 字段选择器 </param>
        public virtual TSet Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            Queue.ExpBuilder.AddOrderBy(exp: desc, isAsc: false);
            return (TSet)this;
        }

        /// <summary>
        ///     正序查询
        /// </summary>
        /// <typeparam name="TKey"> 实体类属性类型 </typeparam>
        /// <param name="asc"> 字段选择器 </param>
        public virtual TSet Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            Queue.ExpBuilder.AddOrderBy(exp: asc, isAsc: true);
            return (TSet)this;
        }

        #endregion

        #region ToTable

        /// <summary> 查询多条记录 </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <param name="isRand"> 返回当前条件下随机的数据 </param>
        public virtual DataTable ToTable(int top = 0, bool isDistinct = false, bool isRand = false) => QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.ToTable(callMethod: $"{typeof(TEntity).Name}.ToTable", sqlParam: queue.SqlBuilder.ToList(top: top, isDistinct: isDistinct, isRand: isRand)), joinSoftDeleteCondition: true);

        /// <summary> 异步查询多条记录 </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <param name="isRand"> 返回当前条件下随机的数据 </param>
        public virtual Task<DataTable> ToTableAsync(int top = 0, bool isDistinct = false, bool isRand = false) => QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ToTableAsync(callMethod: $"{typeof(TEntity).Name}.ToTableAsync", sqlParam: queue.SqlBuilder.ToList(top: top, isDistinct: isDistinct, isRand: isRand)), joinSoftDeleteCondition: true);

        /// <summary> 查询多条记录 </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <returns> </returns>
        public virtual DataTable ToTable(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 计算总页数
            Check.IsTure(isTrue: pageIndex < 1, parameterName: $"参数{nameof(pageIndex)}，不能小于1");
            Check.IsTure(isTrue: pageSize  < 1, parameterName: $"参数{nameof(pageSize)}，不能小于1");

            return QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.ToTable(callMethod: $"{typeof(TEntity).Name}.ToTable", sqlParam: queue.SqlBuilder.ToList(pageSize: pageSize, pageIndex: pageIndex, isDistinct: isDistinct)), joinSoftDeleteCondition: true);
        }

        /// <summary> 异步查询多条记录 </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <returns> </returns>
        public virtual Task<DataTable> ToTableAsync(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 计算总页数
            Check.IsTure(isTrue: pageIndex < 1, parameterName: $"参数{nameof(pageIndex)}，不能小于1");
            Check.IsTure(isTrue: pageSize  < 1, parameterName: $"参数{nameof(pageSize)}，不能小于1");

            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ToTableAsync(callMethod: $"{typeof(TEntity).Name}.ToTableAsync", sqlParam: queue.SqlBuilder.ToList(pageSize: pageSize, pageIndex: pageIndex, isDistinct: isDistinct)), joinSoftDeleteCondition: true);
        }

        /// <summary> 查询多条记录 </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="recordCount"> 总记录数量 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        public virtual DataTable ToTable(int pageSize, int pageIndex, out int recordCount, bool isDistinct = false)
        {
            // 计算总页数
            Check.IsTure(isTrue: pageIndex < 1, parameterName: $"参数{nameof(pageIndex)}，不能小于1");
            Check.IsTure(isTrue: pageSize  < 1, parameterName: $"参数{nameof(pageSize)}，不能小于1");

            var queue = Queue;
            recordCount = Count();
            Queue.Copy(queue: queue);

            return ToTable(pageSize: pageSize, pageIndex: pageIndex, isDistinct: isDistinct);
        }

        /// <summary> 异步查询多条记录 </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="recordCount"> 总记录数量 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        public virtual Task<DataTable> ToTableAsync(int pageSize, int pageIndex, out int recordCount, bool isDistinct = false)
        {
            // 计算总页数
            Check.IsTure(isTrue: pageIndex < 1, parameterName: $"参数{nameof(pageIndex)}，不能小于1");
            Check.IsTure(isTrue: pageSize  < 1, parameterName: $"参数{nameof(pageSize)}，不能小于1");

            var queue = Queue;
            recordCount = Count();
            Queue.Copy(queue: queue);

            return ToTableAsync(pageSize: pageSize, pageIndex: pageIndex, isDistinct: isDistinct);
        }

        #endregion

        #region ToList

        /// <summary> 查询多条记录 </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <param name="isRand"> 返回当前条件下随机的数据 </param>
        public virtual List<TEntity> ToList(int top = 0, bool isDistinct = false, bool isRand = false) => QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.ToList<TEntity>(callMethod: $"{typeof(TEntity).Name}.ToList", sqlParam: queue.SqlBuilder.ToList(top: top, isDistinct: isDistinct, isRand: isRand)), joinSoftDeleteCondition: true);

        /// <summary> 查询多条记录 </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <param name="isRand"> 返回当前条件下随机的数据 </param>
        public virtual Task<List<TEntity>> ToListAsync(int top = 0, bool isDistinct = false, bool isRand = false) => QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ToListAsync<TEntity>(callMethod: $"{typeof(TEntity).Name}.ToListAsync", sqlParam: queue.SqlBuilder.ToList(top: top, isDistinct: isDistinct, isRand: isRand)), joinSoftDeleteCondition: true);

        /// <summary>
        ///     查询多条记录
        /// </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <returns> </returns>
        public virtual List<TEntity> ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 计算总页数
            Check.IsTure(isTrue: pageIndex < 1, parameterName: $"参数{nameof(pageIndex)}，不能小于1");
            Check.IsTure(isTrue: pageSize  < 1, parameterName: $"参数{nameof(pageSize)}，不能小于1");

            return QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.ToList<TEntity>(callMethod: $"{typeof(TEntity).Name}.ToList", sqlParam: queue.SqlBuilder.ToList(pageSize: pageSize, pageIndex: pageIndex, isDistinct: isDistinct)), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     查询多条记录
        /// </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <returns> </returns>
        public virtual Task<List<TEntity>> ToListAsync(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 计算总页数
            Check.IsTure(isTrue: pageIndex < 1, parameterName: $"参数{nameof(pageIndex)}，不能小于1");
            Check.IsTure(isTrue: pageSize  < 1, parameterName: $"参数{nameof(pageSize)}，不能小于1");

            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ToListAsync<TEntity>(callMethod: $"{typeof(TEntity).Name}.ToListAsync", sqlParam: queue.SqlBuilder.ToList(pageSize: pageSize, pageIndex: pageIndex, isDistinct: isDistinct)), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     查询分页记录
        /// </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        public virtual PageList<TEntity> ToPageList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 计算总页数
            Check.IsTure(isTrue: pageIndex < 1, parameterName: $"参数{nameof(pageIndex)}，不能小于1");
            Check.IsTure(isTrue: pageSize  < 1, parameterName: $"参数{nameof(pageSize)}，不能小于1");

            var queue       = Queue;
            var recordCount = Count();
            Queue.Copy(queue: queue);

            var lst = ToList(pageSize: pageSize, pageIndex: pageIndex, isDistinct: isDistinct);
            return new PageList<TEntity>(lst, recordCount);
        }

        /// <summary>
        ///     查询分页记录
        /// </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        public virtual async Task<PageList<TEntity>> ToPageListAsync(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 计算总页数
            Check.IsTure(isTrue: pageIndex < 1, parameterName: $"参数{nameof(pageIndex)}，不能小于1");
            Check.IsTure(isTrue: pageSize  < 1, parameterName: $"参数{nameof(pageSize)}，不能小于1");

            var queue       = Queue;
            var recordCount = await CountAsync();
            Queue.Copy(queue: queue);

            var lst = await ToListAsync(pageSize: pageSize, pageIndex: pageIndex, isDistinct: isDistinct);
            return new PageList<TEntity>(lst, recordCount);
        }

        #endregion

        #region ToSelectList

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="select"> 字段选择器 </param>
        public virtual List<T> ToSelectList<T>(Expression<Func<TEntity, T>> select) => ToSelectList(top: 0, select: select);

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="select"> 字段选择器 </param>
        public virtual Task<List<T>> ToSelectListAsync<T>(Expression<Func<TEntity, T>> select) => ToSelectListAsync(top: 0, select: select);

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="select"> 字段选择器 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        public virtual List<T> ToSelectList<T>(int top, Expression<Func<TEntity, T>> select) => Select(select: select).ToList(top: top).Select(selector: select.Compile()).ToList();

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="select"> 字段选择器 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        public virtual async Task<List<T>> ToSelectListAsync<T>(int top, Expression<Func<TEntity, T>> select)
        {
            var lst = await Select(select: select).ToListAsync(top: top);
            return lst.Select(selector: select.Compile()).ToList();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual List<T> ToSelectList<T>(List<T> lstIDs, Expression<Func<TEntity, T>> select, string memberName = null)
        {
            Where(lstvValues: lstIDs, memberName: memberName);
            return ToSelectList(select: select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<List<T>> ToSelectListAsync<T>(List<T> lstIDs, Expression<Func<TEntity, T>> select, string memberName = null)
        {
            Where(lstvValues: lstIDs, memberName: memberName);
            return ToSelectListAsync(select: select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <param name="top"> 限制显示的数量 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual List<T> ToSelectList<T>(List<T> lstIDs, int top, Expression<Func<TEntity, T>> select, string memberName = null)
        {
            Where(lstvValues: lstIDs, memberName: memberName);
            return ToSelectList(top: top, select: select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <param name="top"> 限制显示的数量 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<List<T>> ToSelectListAsync<T>(List<T> lstIDs, int top, Expression<Func<TEntity, T>> select, string memberName = null)
        {
            Where(lstvValues: lstIDs, memberName: memberName);
            return ToSelectListAsync(top: top, select: select);
        }

        #endregion

        #region ToSelectArray

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="select"> 字段选择器 </param>
        public virtual T[] ToSelectArray<T>(Expression<Func<TEntity, T>> select) => ToSelectArray(top: 0, select: select);

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="select"> 字段选择器 </param>
        public virtual Task<T[]> ToSelectArrayAsync<T>(Expression<Func<TEntity, T>> select) => ToSelectArrayAsync(top: 0, select: select);

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="select"> 字段选择器 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        public virtual T[] ToSelectArray<T>(int top, Expression<Func<TEntity, T>> select) => Select(select: select).ToList(top: top).Select(selector: select.Compile()).ToArray();

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="select"> 字段选择器 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        public virtual async Task<T[]> ToSelectArrayAsync<T>(int top, Expression<Func<TEntity, T>> select)
        {
            var lst = await Select(select: select).ToListAsync(top: top);
            return lst.Select(selector: select.Compile()).ToArray();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual T[] ToSelectArray<T>(T[] lstIDs, Expression<Func<TEntity, T>> select, string memberName = null)
        {
            Where(value: lstIDs, memberName: memberName);
            return ToSelectArray(select: select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<T[]> ToSelectArrayAsync<T>(T[] lstIDs, Expression<Func<TEntity, T>> select, string memberName = null)
        {
            Where(value: lstIDs, memberName: memberName);
            return ToSelectArrayAsync(select: select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <param name="top"> 限制显示的数量 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual T[] ToSelectArray<T>(T[] lstIDs, int top, Expression<Func<TEntity, T>> select, string memberName = null)
        {
            Where(value: lstIDs, memberName: memberName);
            return ToSelectArray(top: top, select: select);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> o => IDs.Contains(o.ID) </param>
        /// <param name="top"> 限制显示的数量 </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 实体类的属性 </typeparam>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<T[]> ToSelectArrayAsync<T>(T[] lstIDs, int top, Expression<Func<TEntity, T>> select, string memberName = null)
        {
            Where(value: lstIDs, memberName: memberName);
            return ToSelectArrayAsync(top: top, select: select);
        }

        #endregion

        #region ToEntity

        /// <summary>
        ///     查询单条记录
        /// </summary>
        public virtual TEntity ToEntity() => QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.ToEntity<TEntity>(callMethod: $"{typeof(TEntity).Name}.ToEntity", sqlParam: queue.SqlBuilder.ToEntity()), joinSoftDeleteCondition: true);

        /// <summary>
        ///     查询单条记录
        /// </summary>
        public virtual Task<TEntity> ToEntityAsync() => QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.ToEntityAsync<TEntity>(callMethod: $"{typeof(TEntity).Name}.ToEntityAsync", sqlParam: queue.SqlBuilder.ToEntity()), joinSoftDeleteCondition: true);

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual TEntity ToEntity<T>(T ID, string memberName = null) where T : struct
        {
            Where(value: ID, memberName: memberName);
            return ToEntity();
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<TEntity> ToEntityAsync<T>(T ID, string memberName = null) where T : struct
        {
            Where(value: ID, memberName: memberName);
            return ToEntityAsync();
        }

        #endregion

        #region Count

        /// <summary>
        ///     查询数量
        /// </summary>
        public virtual int Count(bool isDistinct = false, bool isRand = false) => QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.GetValue<int>(callMethod: $"{typeof(TEntity).Name}.Count", sqlParam: queue.SqlBuilder.Count()), joinSoftDeleteCondition: true);

        /// <summary>
        ///     查询数量
        /// </summary>
        public virtual Task<int> CountAsync(bool isDistinct = false, bool isRand = false) => QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.GetValueAsync<int>(callMethod: $"{typeof(TEntity).Name}.CountAsync", sqlParam: queue.SqlBuilder.Count()), joinSoftDeleteCondition: true);

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual int Count<T>(T ID, string memberName = null) where T : struct
        {
            Where(value: ID, memberName: memberName);
            return Count();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<int> CountAsync<T>(T ID, string memberName = null) where T : struct
        {
            Where(value: ID, memberName: memberName);
            return CountAsync();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual int Count<T>(List<T> lstIDs, string memberName = null) where T : struct
        {
            Where(lstvValues: lstIDs, memberName: memberName);
            return Count();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<int> CountAsync<T>(List<T> lstIDs, string memberName = null) where T : struct
        {
            Where(lstvValues: lstIDs, memberName: memberName);
            return CountAsync();
        }

        #endregion

        #region IsHaving

        /// <summary>
        ///     查询数据是否存在
        /// </summary>
        public virtual bool IsHaving() => Count() > 0;

        /// <summary>
        ///     查询数据是否存在
        /// </summary>
        public virtual async Task<bool> IsHavingAsync() => await CountAsync() > 0;

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID == ID 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual bool IsHaving<T>(T ID, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).IsHaving();

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID == ID 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<bool> IsHavingAsync<T>(T ID, string memberName = null) where T : struct => Where(value: ID, memberName: memberName).IsHavingAsync();

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual bool IsHaving<T>(List<T> lstIDs, string memberName = null) => Where(lstvValues: lstIDs, memberName: memberName).IsHaving();

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <typeparam name="T"> ID </typeparam>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<bool> IsHavingAsync<T>(List<T> lstIDs, string memberName = null) => Where(lstvValues: lstIDs, memberName: memberName).IsHavingAsync();

        #endregion

        #region GetValue

        /// <summary>
        ///     查询单个值
        /// </summary>
        public virtual T GetValue<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default)
        {
            if (fieldName == null) throw new ArgumentNullException(paramName: "fieldName", message: "查询Value操作时，fieldName参数不能为空！");

            Select(select: fieldName);
            return QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.GetValue(callMethod: $"{typeof(TEntity).Name}.GetValue", sqlParam: queue.SqlBuilder.GetValue(), defValue: defValue), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     查询单个值
        /// </summary>
        public virtual Task<T> GetValueAsync<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default)
        {
            if (fieldName == null) throw new ArgumentNullException(paramName: "fieldName", message: "查询Value操作时，fieldName参数不能为空！");

            Select(select: fieldName);
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.GetValueAsync(callMethod: $"{typeof(TEntity).Name}.GetValueAsync", sqlParam: queue.SqlBuilder.GetValue(), defValue: defValue), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     查询单个值
        /// </summary>
        /// <typeparam name="T1"> ID </typeparam>
        /// <typeparam name="T2"> 字段类型 </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <param name="fieldName"> 筛选字段 </param>
        /// <param name="defValue"> 不存在时默认值 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual T2 GetValue<T1, T2>(T1 ID, Expression<Func<TEntity, T2>> fieldName, T2 defValue = default, string memberName = null) where T1 : struct => Where(value: ID, memberName: memberName).GetValue(fieldName: fieldName, defValue: defValue);

        /// <summary>
        ///     查询单个值
        /// </summary>
        /// <typeparam name="T1"> ID </typeparam>
        /// <typeparam name="T2"> 字段类型 </typeparam>
        /// <param name="ID"> 条件，等同于：o=>o.ID.Equals(ID) 的操作 </param>
        /// <param name="fieldName"> 筛选字段 </param>
        /// <param name="defValue"> 不存在时默认值 </param>
        /// <param name="memberName"> 条件字段名称，如为Null，默认为主键字段 </param>
        public virtual Task<T2> GetValueAsync<T1, T2>(T1 ID, Expression<Func<TEntity, T2>> fieldName, T2 defValue = default, string memberName = null) where T1 : struct => Where(value: ID, memberName: memberName).GetValueAsync(fieldName: fieldName, defValue: defValue);

        #endregion

        #region 聚合

        /// <summary>
        ///     累计和
        /// </summary>
        public virtual T Sum<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default)
        {
            if (fieldName == null) throw new ArgumentNullException(paramName: "fieldName", message: "查询Sum操作时，fieldName参数不能为空！");

            Select(select: fieldName);
            return QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.GetValue(callMethod: $"{typeof(TEntity).Name}.Sum", sqlParam: queue.SqlBuilder.Sum(), defValue: defValue), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     累计和
        /// </summary>
        public virtual Task<T> SumAsync<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default)
        {
            if (fieldName == null) throw new ArgumentNullException(paramName: "fieldName", message: "查询Sum操作时，fieldName参数不能为空！");

            Select(select: fieldName);
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.GetValueAsync(callMethod: $"{typeof(TEntity).Name}.SumAsync", sqlParam: queue.SqlBuilder.Sum(), defValue: defValue), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     查询最大数
        /// </summary>
        public virtual T Max<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default) where T : struct
        {
            if (fieldName == null) throw new ArgumentNullException(paramName: "fieldName", message: "查询Max操作时，fieldName参数不能为空！");

            Select(select: fieldName);
            return QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.GetValue(callMethod: $"{typeof(TEntity).Name}.Max", sqlParam: queue.SqlBuilder.Max(), defValue: defValue), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     查询最大数
        /// </summary>
        public virtual Task<T> MaxAsync<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default) where T : struct
        {
            if (fieldName == null) throw new ArgumentNullException(paramName: "fieldName", message: "查询Max操作时，fieldName参数不能为空！");

            Select(select: fieldName);
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.GetValueAsync(callMethod: $"{typeof(TEntity).Name}.MaxAsync", sqlParam: queue.SqlBuilder.Max(), defValue: defValue), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     查询最小数
        /// </summary>
        public virtual T Min<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default) where T : struct
        {
            if (fieldName == null) throw new ArgumentNullException(paramName: "fieldName", message: "查询Min操作时，fieldName参数不能为空！");

            Select(select: fieldName);
            return QueueManger.Commit(map: SetMap, act: queue => Context.Executeor.GetValue(callMethod: $"{typeof(TEntity).Name}.Min", sqlParam: queue.SqlBuilder.Min(), defValue: defValue), joinSoftDeleteCondition: true);
        }

        /// <summary>
        ///     查询最小数
        /// </summary>
        public virtual Task<T> MinAsync<T>(Expression<Func<TEntity, T>> fieldName, T defValue = default) where T : struct
        {
            if (fieldName == null) throw new ArgumentNullException(paramName: "fieldName", message: "查询Min操作时，fieldName参数不能为空！");

            Select(select: fieldName);
            return QueueManger.CommitAsync(map: SetMap, act: queue => Context.Executeor.GetValueAsync(callMethod: $"{typeof(TEntity).Name}.MinAsync", sqlParam: queue.SqlBuilder.Min(), defValue: defValue), joinSoftDeleteCondition: true);
        }

        #endregion
    }
}