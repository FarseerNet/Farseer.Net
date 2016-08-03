using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class Extend
    {
        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this IEnumerable<int> lst, int? value)
        {
            return Enumerable.Contains(lst, value.GetValueOrDefault());
        }
        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this IEnumerable<long> lst, long? value)
        {
            return Enumerable.Contains(lst, value.GetValueOrDefault());
        }

        /// <summary>
        ///     将List转换成字符串
        /// </summary>
        /// <param name="lst">要拼接的LIST</param>
        /// <param name="sign">分隔符</param>
        public static string ToString(this IEnumerable lst, string sign = ",")
        {
            return ConvertHelper.ToString(lst, sign);
        }

        /// <summary>
        ///     对List，按splitCount大小进行分组
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="splitCount">每组大小</param>
        public static List<IEnumerable<TEntity>> Split<TEntity>(this IEnumerable<TEntity> lst, int splitCount)
        {
            var lstGroup = new List<IEnumerable<TEntity>>();
            if (lst == null) { return lstGroup; }

            var groupLength = (int)Math.Ceiling(((decimal)lst.Count() / splitCount));
            for (var pageIndex = 0; pageIndex < groupLength; pageIndex++)
            {
                lstGroup.Add(lst.Skip(splitCount * pageIndex).Take(splitCount));
            }
            return lstGroup;
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, int pageSize, int pageIndex = 1)
        {
            if (pageSize == 0) { return lst.ToList(); }

            #region 计算总页数

            var allCurrentPage = 0;
            var recordCount = lst.Count();
            if (pageIndex < 1)
            {
                pageIndex = 1;
                return lst.Take(pageSize).ToList();
            }
            if (pageSize < 1) { pageSize = 10; }

            if (pageSize != 0)
            {
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            if (pageIndex > allCurrentPage) { pageIndex = allCurrentPage; }

            #endregion

            return lst.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        /// <param name="recordCount">总数量</param>
        public static List<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, int pageSize, int pageIndex, out int recordCount)
        {
            var enumerable = lst as IList<TEntity> ?? lst.ToList();
            recordCount = enumerable.Count;
            return ToList(enumerable, pageSize, pageIndex);
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, Func<TEntity, T> select)
        {
            return lst?.Select(select).ToList();
        }
    }
}