﻿using System;
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
    public static async Task<TDestination> MapAsync<TDestination, TSource>(this Task<TSource> sourceTask, Action<TDestination, TSource> mapRule = null)
    {
        if (sourceTask == null) return default;
        var source = await sourceTask;
        var dest   = MapperHelper.Mapper.Map<TDestination>(source: source);
        if (mapRule != null) mapRule(dest, source);
        return dest;
    }
}