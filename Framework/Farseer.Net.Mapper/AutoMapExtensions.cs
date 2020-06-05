using FS.Mapper;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class AutoMapExtensions
    {
        public static TDestination Map<TDestination>(this object source)
        {
            return AutoMapperHelper.Mapper.Map<TDestination>(source);
        }

        public static TDestination Map<TSource,TDestination>(this TSource source,TDestination destination)
        {
            return AutoMapperHelper.Mapper.Map(source, destination);
        }
    }
}
