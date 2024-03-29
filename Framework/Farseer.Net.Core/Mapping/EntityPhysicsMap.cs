﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Collections.Pooled;

namespace FS.Core.Mapping
{
    /// <summary>
    ///     普通实体类的 映射关系
    /// </summary>
    public class EntityPhysicsMap : IDisposable
    {
        /// <summary>
        ///     缓存类型
        /// </summary>
        private static readonly ConcurrentDictionary<Type, EntityPhysicsMap> Cache = new();
        private static readonly object objLock = new();
        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public readonly PooledDictionary<PropertyInfo, EntityPropertyMap> MapList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type"> 实体类Type </param>
        private EntityPhysicsMap(Type type)
        {
            Type    = type;
            MapList = new();

            // 循环Set的字段
            foreach (var propertyInfo in Type.GetProperties())
            {
                var modelAtt = new EntityPropertyMap { ValidationList = new() };

                var attrs = propertyInfo.GetCustomAttributes(inherit: false);

                // 先获取描述特性
                var displayAtt = attrs.FirstOrDefault(predicate: o => o is DisplayAttribute);
                modelAtt.Display = displayAtt == null ? new DisplayAttribute { Name = propertyInfo.Name } : (DisplayAttribute)displayAtt;
                if (string.IsNullOrEmpty(value: modelAtt.Display.Name)) modelAtt.Display.Name = propertyInfo.Name;

                // 找出所有验证特性
                var vals = attrs.Where(predicate: o => o is ValidationAttribute).ToList();
                if (vals == null) return;

                // 遍历所有特性
                foreach (var item in vals)
                {
                    // 字符串长度
                    if (item is StringLengthAttribute stringLengthAtt)
                    {
                        if (string.IsNullOrEmpty(value: stringLengthAtt.ErrorMessage))
                        {
                            if (stringLengthAtt.MinimumLength > 0 && stringLengthAtt.MaximumLength > 0)
                                stringLengthAtt.ErrorMessage = $"{modelAtt.Display.Name}，长度范围必须为：{stringLengthAtt.MinimumLength} - {stringLengthAtt.MaximumLength} 个字符之间！";
                            else if (stringLengthAtt.MaximumLength > 0)
                                stringLengthAtt.ErrorMessage = $"{modelAtt.Display.Name}，长度不能大于{stringLengthAtt.MaximumLength}个字符！";
                            else
                                stringLengthAtt.ErrorMessage = $"{modelAtt.Display.Name}，长度不能小于{stringLengthAtt.MinimumLength}个字符！";
                        }
                    }

                    // 是否必填
                    else if (item is RequiredAttribute requiredAtt)
                    {
                        if (string.IsNullOrEmpty(value: requiredAtt.ErrorMessage)) requiredAtt.ErrorMessage = $"{modelAtt.Display.Name}，不能为空！";
                    }
                    // 范围
                    else if (item is RangeAttribute rangeAtt)
                    {
                        if (string.IsNullOrEmpty(value: rangeAtt.ErrorMessage))
                        {
                            decimal.TryParse(s: rangeAtt.Minimum.ToString(), result: out var minnum);
                            decimal.TryParse(s: rangeAtt.Minimum.ToString(), result: out var maximum);

                            if (minnum > 0 && maximum > 0)
                                rangeAtt.ErrorMessage = $"{modelAtt.Display.Name}，的值范围必须为：{minnum} - {maximum} 之间！";
                            else if (maximum > 0)
                                rangeAtt.ErrorMessage = $"{modelAtt.Display.Name}，的值不能大于{maximum}！";
                            else
                                rangeAtt.ErrorMessage = $"{modelAtt.Display.Name}，的值不能小于{minnum}！";
                        }
                    }

                    modelAtt.ValidationList.Add(item: (ValidationAttribute)item);
                }

                MapList.Add(key: propertyInfo, value: modelAtt);
            }
        }

        /// <summary>
        ///     类型
        /// </summary>
        internal Type Type { get; }

        /// <summary>
        ///     普通实体类的 映射关系
        /// </summary>
        public static EntityPhysicsMap Map(Type type)
        {
            // 不存在缓存，则加入
            if (Cache.ContainsKey(key: type)) return Cache[key: type];

            lock (objLock)
            {
                Cache.TryAdd(key: type, value: new EntityPhysicsMap(type: type));
            }
            return Cache[key: type];
        }
        
        public void Dispose()
        {
            foreach (var entityPropertyMap in MapList)
            {
                entityPropertyMap.Value.ValidationList.Dispose();
            }
            MapList.Dispose();
        }
    }
}