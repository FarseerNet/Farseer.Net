using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.Core;
using FS.Mapper;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class MapExtend
    {
        public static PageList<TDestination> Map<TDestination>(this IPageList source) where TDestination : class
        {
            return source == null ? null : new PageList<TDestination>(source.GetList().Map<TDestination>(), source.RecordCount);
        }
        public static async Task<PageList<TDestination>> MapAsync<TDestination, TSource>(this Task<PageList<TSource>> sourceTask) where TSource : class where TDestination : class
        {
            var source = await sourceTask;
            return source == null ? null : new PageList<TDestination>(source.List.Map<TDestination>(), source.RecordCount);
        }

        public static PageList<TDestination> Map<TDestination, TSource>(this PageList<TSource> source, Action<TDestination, TSource> mapRule) where TSource : class where TDestination : class
        {
            return source == null ? null : new PageList<TDestination>(source.List.Map(mapRule), source.RecordCount);
        }
        public static async Task<PageList<TDestination>> MapAsync<TDestination, TSource>(this Task<PageList<TSource>> sourceTask, Action<TDestination, TSource> mapRule) where TSource : class where TDestination : class
        {
            var source = await sourceTask;
            return source == null ? null : new PageList<TDestination>(source.List.Map(mapRule), source.RecordCount);
        }

        public static List<TDestination> Map<TDestination>(this IEnumerable source) => source == null ? null : MapperHelper.Mapper.Map<List<TDestination>>(source: source);
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
}