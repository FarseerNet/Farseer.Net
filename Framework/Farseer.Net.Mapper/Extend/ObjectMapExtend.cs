using System;
using System.Threading.Tasks;
using FS.Mapper;
using Mapster;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class ObjectMapExtend
{
    public static TDestination Map<TDestination>(this object source) => source == null ? default : source.Adapt<TDestination>();
    public static TDestination Map<TDestination, TSource>(this TSource source, Action<TDestination, TSource> mapRule)
    {
        if (source == null) return default;
        var dest = source.Adapt<TDestination>();;
        mapRule(dest, source);
        return dest;
    }
}