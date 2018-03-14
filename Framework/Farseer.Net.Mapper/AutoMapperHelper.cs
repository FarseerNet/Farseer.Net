using System;
using System.Linq;
using System.Reflection;
using AutoMapper.Configuration;

namespace FS.Mapper
{
    public class AutoMapperHelper
    {
        /// <summary>
        /// 初始化AutoMapper
        /// </summary>
        /// <param name="typeArr"></param>
        public static void CreateMap(Type[] typeArr)
        {
            var cfg = new MapperConfigurationExpression();
            foreach (var type in typeArr)
            {
                CreateMap<AutoMapFromAttribute>(type, cfg);
                CreateMap<AutoMapToAttribute>(type, cfg);
                CreateMap<AutoMapAttribute>(type, cfg);
            }
            AutoMapper.Mapper.Initialize(cfg);
        }

        /// <summary>
        /// 根据Map方向，完成类型间的Map
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="cfg"></param>
        public static void CreateMap<TAttribute>(Type type, MapperConfigurationExpression cfg) where TAttribute : AutoMapAttribute
        {
            if (!type.IsDefined(typeof(TAttribute)))
            {
                return;
            }
            foreach (var autoMapToAttribute in type.GetCustomAttributes<TAttribute>())
            {
                if (autoMapToAttribute.TargetTypes == null || autoMapToAttribute.TargetTypes.Count() <= 0)
                {
                    continue;
                }
                foreach (var targetType in autoMapToAttribute.TargetTypes)
                {
                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.To))
                    {
                        cfg.CreateMap(type, targetType);
                    }

                    if (autoMapToAttribute.Direction.HasFlag(AutoMapDirection.From))
                    {
                        cfg.CreateMap(targetType, type);
                    }
                }
            }
        }
    }
}
