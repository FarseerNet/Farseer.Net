using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class AdaptAsyncExtend
{
    public static async Task<TDestination> AdaptAsync<TDestination, TSource>(this Task<TSource> sourceTask)
    {
        if (sourceTask == null) return default;
        var source = await sourceTask;
        return source.Adapt<TDestination>();
    }
    
    public static async Task<List<TDestination>> AdaptAsync<TDestination, TSource>(this Task<IEnumerable<TSource>> sourceTask, Action<TDestination, TSource> mapRule = null)
    {
        var sources = await sourceTask;
        if (sources == null) return default;
        if (mapRule == null) return sources.Adapt<List<TDestination>>();

        var lst = new List<TDestination>();
        foreach (var source in sources)
        {
            var dest = source.Adapt<TDestination>();
            mapRule(dest, source);
            lst.Add(dest);
        }
        return lst;
    }
    
    public static async Task<List<TDestination>> AdaptAsync<TDestination, TSource>(this Task<List<TSource>> sourceTask, Action<TDestination, TSource> mapRule = null)
    {
        var sources = await sourceTask;
        if (sources == null) return default;
        if (mapRule == null) return sources.Adapt<List<TDestination>>();

        var lst = new List<TDestination>();
        foreach (var source in sources)
        {
            var dest = source.Adapt<TDestination>();
            mapRule(dest, source);
            lst.Add(dest);
        }
        return lst;
    }
    
    public static async Task<List<TDestination>> AdaptAsync<TDestination, TSource>(this IAsyncEnumerable<TSource> lstSourceAsync, Action<TDestination, TSource> mapRule = null)
    {
        if (lstSourceAsync == null) return default;
        if (mapRule == null)
        {
            var lstSource = new List<TSource>();
            await foreach (var source in lstSourceAsync)
            {
                lstSource.Add(source);
            }
            return lstSource.Adapt<List<TDestination>>();
        }

        var lst = new List<TDestination>();
        await foreach (var source in lstSourceAsync)
        {
            var dest = source.Adapt<TDestination>();
            mapRule(dest, source);
            lst.Add(dest);
        }
        return lst;
    }
}