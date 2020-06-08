using System.Collections;
using System.Collections.Generic;
using FS.Mapper;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class MapExtend
    {
       public static List<TDestination> Map<TDestination>(this IEnumerable source) => MapperHelper.Mapper.Map<List<TDestination>>(source);

       public static TDestination Map<TDestination>(this object source) => MapperHelper.Mapper.Map<TDestination>(source);

       public static TDestination Map<TSource,TDestination>(this TSource source,TDestination destination) => MapperHelper.Mapper.Map(source, destination);
    }
}