using System;
using System.Collections.Generic;
using System.Linq;
using Farseer.Net.Core;

// ReSharper disable once CheckNamespace
namespace Farseer.Net.Extends
{
    public static partial class SqlExtend
    {
        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, List<int> lstIDs, int pageSize, int pageIndex = 1) where TEntity : IEntity
        {
            return lst.Where(o => lstIDs.Contains(o.ID.GetValueOrDefault())).ToList(pageSize, pageIndex);
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, List<int> lstIDs, int pageSize, int pageIndex, out int recordCount) where TEntity : IEntity
        {
            return lst.Where(o => lstIDs.Contains(o.ID.GetValueOrDefault())).ToList(pageSize, pageIndex, out recordCount);
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="lst">要获取值的列表</param>
        /// <param name="ID">当前ID</param>
        public static TEntity ToNextInfo<TEntity>(this IEnumerable<TEntity> lst, int ID) where TEntity : IEntity
        {
            return lst.Where(o => o.ID > ID).OrderBy(o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="lst">要获取值的列表</param>
        /// <param name="ID">当前ID</param>
        public static TEntity ToPreviousInfo<TEntity>(this IEnumerable<TEntity> lst, int ID) where TEntity : IEntity
        {
            return lst.Where(o => o.ID < ID).OrderByDescending(o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     获取List列表
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs) where TEntity : IEntity
        {
            return lst.Where(o => lstIDs.Contains(o.ID)).ToList();
        }

        /// <summary>
        ///     获取Info
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID的操作</param>
        public static TEntity ToEntity<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.FirstOrDefault(o => o.ID == ID);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static int Count<TEntity>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs) where TEntity : IEntity
        {
            return lst.Count(o => lstIDs.Contains(o.ID));
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID 的操作</param>
        public static int Count<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.Count(o => o.ID == ID);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static bool IsHaving<TEntity>(this IEnumerable<TEntity> lst, int? ID) where TEntity : IEntity
        {
            return lst.Count(o => o.ID == ID) > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="lst">List列表</param>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static bool IsHaving<TEntity>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs) where TEntity : IEntity
        {
            return lst.Any(o => lstIDs.Contains(o.ID));
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs, Func<TEntity, T> select) where TEntity : IEntity
        {
            return lst.Where(o => lstIDs.Contains(o.ID)).ToSelectList(@select);
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="top">显示的数量限定</param>
        /// <param name="select">字段选择器</param>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, IEnumerable<int> lstIDs, int top, Func<TEntity, T> select) where TEntity : IEntity
        {
            return lst.Where(o => lstIDs.Contains(o.ID)).Take(top).ToSelectList(@select);
        }
    }
}