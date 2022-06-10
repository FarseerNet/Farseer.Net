using System;
using System.Threading.Tasks;
using Collections.Pooled;
using Mapster;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class PooledListMapExtend
{
    public static async Task<PooledList<TDestination>> MapAsync<TDestination, TSource>(this Task<PooledList<TSource>> sourceTask, Action<TDestination, TSource> mapRule = null)
    {
        var sources = await sourceTask;
        if (sources == null) return default;
        if (mapRule == null) return sources.Adapt<PooledList<TDestination>>();

        var lst = new PooledList<TDestination>();
        foreach (var source in sources)
        {
            var dest = source.Adapt<TDestination>();
            mapRule(dest, source);
            lst.Add(dest);
        }
        return lst;
    }
}