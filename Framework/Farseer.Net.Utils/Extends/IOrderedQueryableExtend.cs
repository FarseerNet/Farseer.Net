using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     对IOrderedQueryable进行分页
        /// </summary>
        /// <typeparam name="TSource">实体类</typeparam>
        /// <param name="source">List列表</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TSource> ToList<TSource>(this IOrderedQueryable<TSource> source, int pageSize, int pageIndex = 1)
        {
            return source.Skip(pageSize*(pageIndex - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        ///     对IOrderedQueryable进行分页
        /// </summary>
        /// <typeparam name="TSource">实体类</typeparam>
        /// <param name="source">List列表</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TSource> ToList<TSource>(this IOrderedQueryable<TSource> source, out int recordCount, int pageSize, int pageIndex = 1)
        {
            recordCount = source.Count();

            #region 计算总页数

            var allCurrentPage = 1;

            if (pageIndex < 1) { pageIndex = 1; }
            if (pageSize < 0) { pageSize = 0; }
            if (pageSize != 0)
            {
                allCurrentPage = (recordCount/pageSize);
                allCurrentPage = ((recordCount%pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            if (pageIndex > allCurrentPage) { pageIndex = allCurrentPage; }

            #endregion

            return source.Skip(pageSize*(pageIndex - 1)).Take(pageSize).ToList();
        }
    }
}