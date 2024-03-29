﻿using System;
using System.Threading.Tasks;
using FS.Mapper;
using Mapster;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class ObjectAsyncMapExtend
{
    public static async Task<TDestination> MapAsync<TDestination, TSource>(this Task<TSource> sourceTask, Action<TDestination, TSource> mapRule = null)
    {
        if (sourceTask == null) return default;
        var source = await sourceTask;
        var dest   = source.Adapt<TDestination>();
        if (mapRule != null) mapRule(dest, source);
        return dest;
    }
}