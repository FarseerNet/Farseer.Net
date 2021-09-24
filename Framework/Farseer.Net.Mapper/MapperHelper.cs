using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;

namespace FS.Mapper
{
    internal class MapperHelper
    {
        public static IMapper Mapper;

        /// <summary>
        ///     初始化AutoMapper
        /// </summary>
        public static void CreateMap(Type[] typeArr)
        {
            var cfg = new MapperConfigurationExpression();
            foreach (var type in typeArr)
            {
                CreateMap<MapFromAttribute>(type: type, cfg: cfg);
                CreateMap<MapToAttribute>(type: type, cfg: cfg);
                CreateMap<MapAttribute>(type: type, cfg: cfg);
            }

            Mapper = new MapperConfiguration(configurationExpression: cfg).CreateMapper();
        }

        /// <summary>
        ///     根据Map方向，完成类型间的Map
        /// </summary>
        public static void CreateMap<TAttribute>(Type type, MapperConfigurationExpression cfg) where TAttribute : MapAttribute
        {
            // 通常type表示po
            if (!type.IsDefined(attributeType: typeof(TAttribute))) return;

            foreach (var autoMapToAttribute in type.GetCustomAttributes<TAttribute>())
            {
                if (autoMapToAttribute.TargetTypes == null || !autoMapToAttribute.TargetTypes.Any()) continue;

                foreach (var targetType in autoMapToAttribute.TargetTypes)
                {
                    if (autoMapToAttribute.Direction.HasFlag(flag: EumMapDirection.To))
                    {
                        var mappingExpression = cfg.CreateMap(sourceType: type, destinationType: targetType);

                        // 找到源实体，是否有MapFieldAttribute特性
                        // 注意的地址，映射到对方时，要取对方的属性名称（目标名称），AutoMapper要求
                        foreach (var propertyInfo in type.GetProperties())
                        {
                            // 目标相同的名称成员
                            var findTarget = Array.Find(array: targetType.GetProperties(), match: o => o.Name.ToLower() == propertyInfo.Name.ToLower());

                            var mapFieldAttribute = propertyInfo.GetCustomAttribute<MapFieldAttribute>();
                            if (mapFieldAttribute == null)
                            {
                                if (findTarget        != null) mapFieldAttribute = findTarget.GetCustomAttribute<MapFieldAttribute>();
                                if (mapFieldAttribute == null) continue;
                            }

                            // 忽略当前字段
                            if (mapFieldAttribute.IsIgnore)
                            {
                                if (findTarget != null) mappingExpression = mappingExpression.ForMember(name: findTarget.Name, memberOptions: opt => opt.Ignore());
                            }

                            // 转换实际的映射名称
                            if (!string.IsNullOrWhiteSpace(value: mapFieldAttribute.FromName))
                            {
                                findTarget = Array.Find(array: targetType.GetProperties(), match: o => o.Name.ToLower() == mapFieldAttribute.FromName.ToLower());
                                if (findTarget != null) mappingExpression = mappingExpression.ForMember(name: findTarget.Name, memberOptions: opt => opt.MapFrom(sourceMembersPath: propertyInfo.Name));
                            }
                        }
                    }

                    if (autoMapToAttribute.Direction.HasFlag(flag: EumMapDirection.From))
                    {
                        var mappingExpression = cfg.CreateMap(sourceType: targetType, destinationType: type);

                        // 找到源实体，是否有MapFieldAttribute特性
                        // 注意的地址，映射到对方时，要取对方的属性名称（目标名称），AutoMapper要求
                        foreach (var propertyInfo in targetType.GetProperties())
                        {
                            var mapFieldAttribute = propertyInfo.GetCustomAttribute<MapFieldAttribute>();
                            if (mapFieldAttribute == null) continue;

                            // 忽略当前字段
                            if (mapFieldAttribute.IsIgnore) mappingExpression = mappingExpression.ForMember(name: propertyInfo.Name, memberOptions: opt => opt.Ignore());

                            // 转换实际的映射名称
                            if (!string.IsNullOrWhiteSpace(value: mapFieldAttribute.FromName))
                            {
                                var findTarget                            = Array.Find(array: targetType.GetProperties(), match: o => o.Name.ToLower() == mapFieldAttribute.FromName.ToLower());
                                if (findTarget != null) mappingExpression = mappingExpression.ForMember(name: propertyInfo.Name, memberOptions: opt => opt.MapFrom(sourceMembersPath: findTarget.Name));
                            }
                        }
                    }
                }
            }
        }
    }
}