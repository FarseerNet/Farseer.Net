using System.Linq;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     数据分页
        /// </summary>
        /// <typeparam name="TSource">实体</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        public static IQueryable<TSource> Split<TSource>(this IQueryable<TSource> source, int pageSize = 20, int pageIndex = 1)
        {
            if (pageIndex < 1) { pageIndex = 1; }
            if (pageSize < 1) { pageSize = 20; }
            return source.Skip(pageSize*(pageIndex - 1)).Take(pageSize);
        }
    }
}