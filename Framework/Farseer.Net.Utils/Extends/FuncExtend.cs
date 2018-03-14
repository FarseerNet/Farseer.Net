using System;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     Func 转换成 Predicate 对象
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="source">源Func对象</param>
        public static Predicate<TEntity> ToPredicate<TEntity>(this Func<TEntity, bool> source) where TEntity : class, new()
        {
            return new Predicate<TEntity>(source);
        }
    }
}