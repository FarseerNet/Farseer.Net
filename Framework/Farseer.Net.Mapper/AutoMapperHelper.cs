using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;

namespace FS.Mapper
{
    internal class AutoMapperHelper
    {
        public static IMapper Mapper;

        /// <summary>
        /// 初始化AutoMapper
        /// </summary>
        public static void CreateMap(Type[] typeArr)
        {
            var cfg = new MapperConfigurationExpression();
            foreach (var type in typeArr)
            {
                CreateMap<AutoMapFromAttribute>(type, cfg);
                CreateMap<AutoMapToAttribute>(type, cfg);
                CreateMap<AutoMapAttribute>(type, cfg);
            }

            Mapper = new MapperConfiguration(cfg).CreateMapper();
        }

        /// <summary>
        /// 根据Map方向，完成类型间的Map
        /// </summary>
        public static void CreateMap<TAttribute>(Type type, MapperConfigurationExpression cfg) where TAttribute : AutoMapAttribute
        {
            if (!type.IsDefined(typeof(TAttribute))) return;

            foreach (var autoMapToAttribute in type.GetCustomAttributes<TAttribute>())
            {
                if (autoMapToAttribute.TargetTypes == null || !autoMapToAttribute.TargetTypes.Any()) continue;
                
                foreach (var targetType in autoMapToAttribute.TargetTypes)
                {
                    if (autoMapToAttribute.Direction.HasFlag(EumAutoMapDirection.To))
                    {
                        cfg.CreateMap(type, targetType);
                    }

                    if (autoMapToAttribute.Direction.HasFlag(EumAutoMapDirection.From))
                    {
                        cfg.CreateMap(targetType, type);
                    }
                }
            }
        }
    }
}