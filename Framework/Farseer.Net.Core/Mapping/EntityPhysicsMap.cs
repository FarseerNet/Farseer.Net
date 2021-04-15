using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace FS.Core.Mapping
{
    /// <summary>
    ///     普通实体类的 映射关系
    /// </summary>
    public class EntityPhysicsMap
    {
        /// <summary>
        /// 缓存类型
        /// </summary>
        private static readonly ConcurrentDictionary<Type, EntityPhysicsMap> Cache = new ConcurrentDictionary<Type, EntityPhysicsMap>();

        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, EntityPropertyMap> MapList;

        /// <summary>
        ///     类型
        /// </summary>
        internal Type Type { get; }

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        private EntityPhysicsMap(Type type)
        {
            Type = type;
            MapList = new Dictionary<PropertyInfo, EntityPropertyMap>();

            // 循环Set的字段
            foreach (var propertyInfo in Type.GetProperties())
            {
                var modelAtt = new EntityPropertyMap { ValidationList = new List<ValidationAttribute>() };

                var attrs = propertyInfo.GetCustomAttributes(false);

                // 先获取描述特性
                var displayAtt = attrs.FirstOrDefault(o => o is DisplayAttribute);
                modelAtt.Display = displayAtt == null ? new DisplayAttribute { Name = propertyInfo.Name } : (DisplayAttribute)displayAtt;
                if (string.IsNullOrEmpty(modelAtt.Display.Name)) { modelAtt.Display.Name = propertyInfo.Name; }

                // 找出所有验证特性
                var vals = attrs.Where(o => o is ValidationAttribute).ToList();
                if (vals == null) { return; }

                // 遍历所有特性
                foreach (var item in vals)
                {
                    // 字符串长度
                    if (item is StringLengthAttribute stringLengthAtt)
                    {
                        if (string.IsNullOrEmpty(stringLengthAtt.ErrorMessage))
                        {
                            if (stringLengthAtt.MinimumLength > 0 && stringLengthAtt.MaximumLength > 0) { stringLengthAtt.ErrorMessage = $"{modelAtt.Display.Name}，长度范围必须为：{stringLengthAtt.MinimumLength} - {stringLengthAtt.MaximumLength} 个字符之间！"; }
                            else if (stringLengthAtt.MaximumLength > 0) { stringLengthAtt.ErrorMessage = $"{modelAtt.Display.Name}，长度不能大于{stringLengthAtt.MaximumLength}个字符！"; }
                            else
                            { stringLengthAtt.ErrorMessage = $"{modelAtt.Display.Name}，长度不能小于{stringLengthAtt.MinimumLength}个字符！"; }
                        }
                    }

                    // 是否必填
                    else if (item is RequiredAttribute requiredAtt)
                    {
                        if (string.IsNullOrEmpty(requiredAtt.ErrorMessage))
                        {
                            requiredAtt.ErrorMessage = $"{modelAtt.Display.Name}，不能为空！";
                        }
                    }
                    // 范围
                    else if (item is RangeAttribute rangeAtt)
                    {
                        if (string.IsNullOrEmpty(rangeAtt.ErrorMessage))
                        {
                            decimal.TryParse(rangeAtt.Minimum.ToString(), out var minnum);
                            decimal.TryParse(rangeAtt.Minimum.ToString(), out var maximum);

                            if (minnum > 0 && maximum > 0) { rangeAtt.ErrorMessage = $"{modelAtt.Display.Name}，的值范围必须为：{minnum} - {maximum} 之间！"; }
                            else if (maximum > 0) { rangeAtt.ErrorMessage = $"{modelAtt.Display.Name}，的值不能大于{maximum}！"; }
                            else
                            { rangeAtt.ErrorMessage = $"{modelAtt.Display.Name}，的值不能小于{minnum}！"; }
                        }
                    }
                    modelAtt.ValidationList.Add((ValidationAttribute)item);
                }

                MapList.Add(propertyInfo, modelAtt);
            }
        }

        /// <summary>
        ///     普通实体类的 映射关系
        /// </summary>
        public static EntityPhysicsMap Map(Type type)
        {
            // 不存在缓存，则加入
            if (!Cache.ContainsKey(type)) Cache.TryAdd(type, new EntityPhysicsMap(type));
            return Cache[type];
        }
    }
}