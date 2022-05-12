using System;
using System.Threading.Tasks;
using FS.Core;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class PageListMapExtend
{
    public static PageList<TDestination> Map<TDestination>(this IPageList source) where TDestination : class
    {
        return source == null ? null : new PageList<TDestination>(source.GetList().Map<TDestination>(), source.RecordCount);
    }

    public static PageList<TDestination> Map<TDestination, TSource>(this PageList<TSource> source, Action<TDestination, TSource> mapRule) where TSource : class where TDestination : class
    {
        return source == null ? null : new PageList<TDestination>(source.List.Map(mapRule), source.RecordCount);
    }
}