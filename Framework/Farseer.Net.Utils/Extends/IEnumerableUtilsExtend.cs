using System;
using System.Collections.Generic;
using System.Linq;
using FS.Core;

// ReSharper disable once CheckNamespace
namespace Farseer.Net.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     复制一个新的List
        /// </summary>
        public static List<T> Copy<T>(this IEnumerable<T> lst)
        {
            if (lst == null) { return new List<T>(); }
            var lstNew = new List<T>(lst.Count());
            lstNew.AddRange(lst);
            return lstNew;
        }

        /// <summary>
        ///     获取Info
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        public static TEntity ToEntity<TEntity>(this IEnumerable<TEntity> lst)
        {
            return lst.FirstOrDefault();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        public static bool IsHaving<TEntity>(this IEnumerable<TEntity> lst)
        {
            return lst.Any();
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <typeparam name="T">ModelInfo</typeparam>
        public static T GetValue<TEntity, T>(this IEnumerable<TEntity> lst, Func<TEntity, T> select, T defValue = default(T))
        {
            if (lst == null) { return defValue; }
            var value = lst.Select(@select).FirstOrDefault();
            return value == null ? defValue : value;
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID 的操作</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <typeparam name="T">ModelInfo</typeparam>
        public static T GetValue<TEntity, T>(this IEnumerable<TEntity> lst, int? ID, Func<TEntity, T> select, T defValue = default(T)) where TEntity : IEntity
        {
            if (lst == null) { return defValue; }
            lst = lst.Where(o => o.ID == ID).ToList();
            if (!lst.Any()) { return defValue; }

            var value = lst.Select(@select).FirstOrDefault();
            return value == null ? defValue : value;
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="top"></param>
        /// <param name="select">字段选择器</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, int top, Func<TEntity, T> select)
        {
            return lst.Select(@select).Take(top).ToList();
        }

        /// <summary>
        ///     克隆List
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this IEnumerable<T> list) where T : ICloneable
        {
            return list?.Select(o => (T) o.Clone()).ToList();
        }

        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this IEnumerable<uint> lst, uint? value)
        {
            return Enumerable.Contains(lst, value.GetValueOrDefault());
        }

        ///// <summary>
        ///// List转换成新的List
        ///// </summary>
        ///// <typeparam name="T1">源类型</typeparam>
        ///// <typeparam name="T2">新的类型</typeparam>
        ///// <param name="lst">源列表</param>
        ///// <param name="defValue">默认值</param>
        //public static List<T2> ToList<T1, T2>(this IEnumerable<T1> lst, T2 defValue) where T1 : struct
        //{
        //    List<T2> lstConvert = new List<T2>();
        //    foreach (var item in lst)
        //    {
        //        lstConvert.Add(item.ConvertType(defValue));
        //    }
        //    return lstConvert;
        //}

        ///// <summary>
        ///// List转换成新的List
        ///// </summary>
        ///// <typeparam name="T1">源类型</typeparam>
        ///// <typeparam name="T2">新的类型</typeparam>
        ///// <param name="lst">源列表</param>
        ///// <param name="func">转换方式</param>
        ///// <returns></returns>
        //public static List<T2> ToList<T1, T2>(this IEnumerable<T1> lst, Func<T1,T2> func) where T1 : struct
        //{
        //    List<T2> lstConvert = new List<T2>();
        //    foreach (var item in lst)
        //    {
        //        lstConvert.Add(func(item));
        //    }
        //    return lstConvert;
        //}


        ///// <summary>
        /////     不重复列表
        ///// </summary>
        ///// <param name="select">字段选择器</param>
        ///// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        ///// <param name="lst">列表</param>
        //public static List<TEntity> ToDistinctList<TEntity, T>(this IEnumerable<TEntity> lst, Func<TEntity, T> select) where TEntity : class
        //{
        //    return lst.Distinct(new InfoComparer<TEntity, T>(select)).ToList();
        //}

        ///// <summary>
        /////     不重复列表
        ///// </summary>
        ///// <param name="select">字段选择器</param>
        ///// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        ///// <param name="lst">列表</param>
        //public static List<TEntity> ToDistinctList<TEntity, T>(this IEnumerable<TEntity> lst, List<int> IDs, Func<TEntity, T> select) where TEntity : class
        //{
        //    return lst.Where(o => IDs.Contains(o.ID)).Distinct(new InfoComparer<TEntity, T>(select)).ToList();
        //}
    }
}