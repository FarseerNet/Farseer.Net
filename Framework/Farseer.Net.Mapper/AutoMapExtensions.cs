using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farseer.Net.Mapper
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
