using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Collections.Pooled;
using Mapster;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class AsyncEnumerableMapAsyncExtend
{
    public static async Task<List<TDestination>> MapAsync<TDestination, TSource>(this IAsyncEnumerable<TSource> lstSourceAsync, Action<TDestination, TSource> mapRule = null)
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