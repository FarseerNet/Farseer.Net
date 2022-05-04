using System;
using System.Threading.Tasks;
using FS.Mapper;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class ObjectMapExtend
{
    public static TDestination Map<TDestination>(this object source) => source == null ? default : MapperHelper.Mapper.Map<TDestination>(source: source);
    public static TDestination Map<TDestination, TSource>(this TSource source, Action<TDestination, TSource> mapRule)
    {
        if (source == null) return default;
        var dest = MapperHelper.Mapper.Map<TDestination>(source: source);
        mapRule(dest, source);
        return dest;
    }
}