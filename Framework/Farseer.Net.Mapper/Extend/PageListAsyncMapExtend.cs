using System;
using System.Threading.Tasks;
using FS.Core;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class PageListAsyncMapExtend
{
    public static async Task<PageList<TDestination>> MapAsync<TDestination, TSource>(this Task<PageList<TSource>> sourceTask) where TSource : class where TDestination : class
    {
        var source = await sourceTask;
        return source == null ? null : new PageList<TDestination>(source.List.Map<TDestination>(), source.RecordCount);
    }

    public static async Task<PageList<TDestination>> MapAsync<TDestination, TSource>(this Task<PageList<TSource>> sourceTask, Action<TDestination, TSource> mapRule) where TSource : class where TDestination : class
    {
        var source = await sourceTask;
        return source == null ? null : new PageList<TDestination>(source.List.Map(mapRule), source.RecordCount);
    }
}