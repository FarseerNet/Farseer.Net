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
}