using System;
using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;
using FS.Core;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class SqlExtend
    {
        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="pageSize"> 每页大小 </param>
        /// <param name="pageIndex"> 索引 </param>
        public static PooledList<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, List<int> lstIDs, int pageSize, int pageIndex = 1) where TEntity : IEntity
        {
            return lst.Where(predicate: o => lstIDs.Contains(item: o.ID.GetValueOrDefault())).ToList(pageSize: pageSize, pageIndex: pageIndex);
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="pageSize"> 每页大小 </param>
        /// <param name="pageIndex"> 索引 </param>
        public static PooledList<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, List<int> lstIDs, int pageSize, int pageIndex, out int recordCount) where TEntity : IEntity
        {
            return lst.Where(predicate: o => lstIDs.Contains(item: o.ID.GetValueOrDefault())).ToList(pageSize: pageSize, pageIndex: pageIndex, recordCount: out recordCount);
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="lst"> 要获取值的列表 </param>
        /// <param name="ID"> 当前ID </param>
        public static TEntity ToNextInfo<TEntity>(this IEnumerable<TEntity> lst, int ID) where TEntity : IEntity
        {
            return lst.Where(predicate: o => o.ID > ID).OrderBy(keySelector: o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="lst"> 要获取值的列表 </param>
        /// <param name="ID"> 当前ID </param>
        public static TEntity ToPreviousInfo<TEntity>(this IEnumerable<TEntity> lst, int ID) where TEntity : IEntity
        {
            return lst.Where(predicate: o => o.ID < ID).OrderByDescending(keySelector: o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     获取List列表
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs) where TEntity : IEntity
        {
            return lst.Where(predicate: o => lstIDs.Contains(value: o.ID)).ToList();
        }

        /// <summary>
        ///     获取Info
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="ID"> 条件，等同于：o=> o.ID == ID的操作 </param>
        public static TEntity ToEntity<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.FirstOrDefault(predicate: o => o.ID == ID);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        public static int Count<TEntity>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs) where TEntity : IEntity
        {
            return lst.Count(predicate: o => lstIDs.Contains(value: o.ID));
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="ID"> 条件，等同于：o=> o.ID == ID 的操作 </param>
        public static int Count<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.Count(predicate: o => o.ID == ID);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="lst"> List列表 </param>
        /// <param name="ID"> 条件，等同于：o=>o.ID == ID 的操作 </param>
        public static bool IsHaving<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.Count(predicate: o => o.ID == ID) > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="lst"> List列表 </param>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        public static bool IsHaving<TEntity>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs) where TEntity : IEntity
        {
            return lst.Any(predicate: o => lstIDs.Contains(value: o.ID));
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="lst"> 列表 </param>
        public static PooledList<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs, Func<TEntity, T> select) where TEntity : IEntity
        {
            return lst.Where(predicate: o => lstIDs.Contains(value: o.ID)).ToSelectList(select: select);
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="top"> 显示的数量限定 </param>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lstIDs"> 条件，等同于：o=> IDs.Contains(o.ID) 的操作 </param>
        /// <param name="lst"> 列表 </param>
        public static PooledList<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs, int top, Func<TEntity, T> select) where TEntity : IEntity
        {
            return lst.Where(predicate: o => lstIDs.Contains(value: o.ID)).Take(count: top).ToSelectList(select: select);
        }
    }
}