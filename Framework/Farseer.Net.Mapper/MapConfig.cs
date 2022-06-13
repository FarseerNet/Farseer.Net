using System;
using System.Linq.Expressions;
using Mapster;

namespace FS.Mapper;

public class MapConfig<TSource, TDestination>
{
    /// <summary>
    /// 设置映射时，双向转换，支持子属性映射
    /// </summary>
    public static void Set()
    {
        TypeAdapterConfig<TSource, TDestination>.NewConfig().Unflattening(true);
    }

    /// <summary>
    /// 设置映射时，双向转换，支持子属性映射
    /// </summary>
    public static void Set<TDestinationMember, TSourceMember>
    (
        Expression<Func<TDestination, TDestinationMember>> member,
        Expression<Func<TSource, TSourceMember>>           source
    )
    {
        TypeAdapterConfig<TSource, TDestination>.NewConfig().Unflattening(true).Map(member,source);
    }
}