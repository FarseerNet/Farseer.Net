using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.Mapper;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class MapExtend
    {
        public static List<TDestination> Map<TDestination>(this          IEnumerable          source) => source == null ? null : MapperHelper.Mapper.Map<List<TDestination>>(source: source);
        public static List<TDestination> Map<TDestination, TSource>(this IEnumerable<TSource> sources, Action<TDestination, TSource> mapRule)
        {
            if (sources == null) return default;
            var lst = new List<TDestination>();
            foreach (var source in sources)
            {
                var dest = MapperHelper.Mapper.Map<TDestination>(source);
                mapRule(dest, source);
                lst.Add(dest);
            }
            return lst;
        }

        public static TDestination Map<TDestination>(this object source)
        {
            return source == null ? default : MapperHelper.Mapper.Map<TDestination>(source: source);
        }
        
        public static TDestination Map<TDestination, TSource>(this TSource source, Action<TDestination, TSource> mapRule)
        {
            if (source == null) return default;
            var dest = MapperHelper.Mapper.Map<TDestination>(source: source);
            mapRule(dest, source);
            return dest;
        }

        public static async Task<List<TDestination>> MapAsync<TDestination, TSource>(this Task<List<TSource>> sourceTask, Action<TDestination, TSource> mapRule = null)
        {
            var sources = await sourceTask;
            if (sources == null) return default;
            if (mapRule == null) return MapperHelper.Mapper.Map<List<TDestination>>(sources);

            var lst = new List<TDestination>();
            foreach (var source in sources)
            {
                var dest = MapperHelper.Mapper.Map<TDestination>(source);
                mapRule(dest, source);
                lst.Add(dest);
            }
            return lst;
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
}