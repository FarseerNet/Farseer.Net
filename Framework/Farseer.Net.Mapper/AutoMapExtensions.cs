namespace FS.Mapper
{
    public static class AutoMapExtensions
    {
        public static TDestination MapTo<TDestination>(this object source)
        {
            return AutoMapper.Mapper.Map<TDestination>(source);
        }

        public static TDestination MapTo<TSource,TDestination>(this TSource source,TDestination destination)
        {
            return AutoMapper.Mapper.Map(source, destination);
        }
    }
}
