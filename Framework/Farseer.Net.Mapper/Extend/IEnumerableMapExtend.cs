using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Mapper;
using Mapster;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class IEnumerableMapExtend
{
    public static IEnumerable<TDestination> Map<TDestination>(this IEnumerable sources)
    {
        if (sources == null) yield break;
        foreach (var source in sources)
        {
            var dest = source.Adapt<TDestination>();
            yield return dest;
        }
    }
    
    public static PooledList<TDestination> Map<TDestination>(this IList sources)
    {
        if (sources == null) return null;

        var lst = new PooledList<TDestination>();
        foreach (var source in sources)
        {
            lst.Add(source.Adapt<TDestination>());
        }
        return lst;
    }

    public static IEnumerable<TDestination> Map<TDestination, TSource>(this IEnumerable<TSource> sources, Action<TDestination, TSource> mapRule)
    {
        if (sources == null) yield break;
        foreach (var source in sources)
        {
            var dest = source.Adapt<TDestination>();
            mapRule(dest, source);
            yield return dest;
        }
    }
}