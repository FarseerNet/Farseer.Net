using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.Mapper;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class ListMapExtend
{
    public static List<TDestination> Map<TDestination>(this IEnumerable source) => source == null ? null : MapperHelper.Mapper.Map<List<TDestination>>(source: source);
    public static IEnumerable<TDestination> Map<TDestination, TSource>(this IEnumerable<TSource> sources, Action<TDestination, TSource> mapRule)
    {
        if (sources == null) yield break;
        foreach (var source in sources)
        {
            var dest = MapperHelper.Mapper.Map<TDestination>(source);
            mapRule(dest, source);
            yield return dest;
        }
    }

    public static async Task<List<TDestination>> MapAsync<TDestination, TSource>(this Task<IEnumerable<TSource>> sourceTask, Action<TDestination, TSource> mapRule = null)
    {
        var sources = await sourceTask;
        if (sources == null) return default;
        if (mapRule == null) return MapperHelper.Mapper.Map<List<TDestination>>(sources);

        var lst = new List<TDestination>();
        foreach (var source in sources)
        {
            var dest = MapperHelper.Mapper.Map<TDestination>(source);
            mapRule(dest, source);
            lst.Add(dest);
        }
        return lst;
    }

    public static async Task<List<TDestination>> MapAsync<TDestination, TSource>(this Task<List<TSource>> sourceTask, Action<TDestination, TSource> mapRule = null)
    {
        var sources = await sourceTask;
        if (sources == null) return default;
        if (mapRule == null) return MapperHelper.Mapper.Map<List<TDestination>>(sources);

        var lst = new List<TDestination>();
        foreach (var source in sources)
        {
            var dest = MapperHelper.Mapper.Map<TDestination>(source);
            mapRule(dest, source);
            lst.Add(dest);
        }
        return lst;
    }
    
    public static async Task<List<TDestination>> MapAsync<TDestination, TSource>(this IAsyncEnumerable<TSource> lstSourceAsync, Action<TDestination, TSource> mapRule = null)
    {
        if (lstSourceAsync == null) return default;
        if (mapRule == null)
        {
            var lstSource = new List<TSource>();
            await foreach (var VARIABLE in lstSourceAsync)
            {
                lstSource.Add(VARIABLE);
            }
            return MapperHelper.Mapper.Map<List<TDestination>>(lstSource);
        }

        var lst = new List<TDestination>();
        await foreach (var source in lstSourceAsync)
        {
            var dest = MapperHelper.Mapper.Map<TDestination>(source);
            mapRule(dest, source);
            lst.Add(dest);
        }
        return lst;
    }
}