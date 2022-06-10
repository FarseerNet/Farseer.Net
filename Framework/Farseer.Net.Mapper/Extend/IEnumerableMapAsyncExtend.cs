using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.Mapper;
using Mapster;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class IEnumerableMapAsyncExtend
{
    public static async Task<List<TDestination>> MapAsync<TDestination, TSource>(this Task<IEnumerable<TSource>> sourceTask, Action<TDestination, TSource> mapRule = null)
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
    
    public static async Task<List<TDestination>> MapAsync<TDestination, TSource>(this Task<List<TSource>> sourceTask, Action<TDestination, TSource> mapRule = null)
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
}