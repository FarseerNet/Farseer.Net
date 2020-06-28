using System.Collections;
using System.Collections.Generic;
using FS.Mapper;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class MapExtend
    {
       public static List<TDestination> Map<TDestination>(this IEnumerable source) => source == null ? null : MapperHelper.Mapper.Map<List<TDestination>>(source);

       public static TDestination Map<TDestination>(this object source) => source == null ? default :MapperHelper.Mapper.Map<TDestination>(source);

       public static TDestination Map<TSource,TDestination>(this TSource source,TDestination destination) => source == null ? default :MapperHelper.Mapper.Map(source, destination);
    }
}