using System;
using System.Collections.Generic;
using System.Reflection;
using FS.Cache;

namespace FS.Map
{
    /// <summary>
    ///     字段 映射关系
    /// </summary>
    public class EntityPhysicsMap
    {
        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public EntityPhysicsMap(Type type)
        {
            Type = type;
            MapList = new Dictionary<PropertyInfo, object[]>();

            // 循环Set的字段
            foreach (var propertyInfo in Type.GetProperties())
            {
                var attrs = propertyInfo.GetCustomAttributes(false);
                MapList.Add(propertyInfo, attrs);
            }
        }

        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, object[]> MapList;

        /// <summary>
        ///     类型
        /// </summary>
        internal Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator EntityPhysicsMap(Type type)
        {
            return EntityPhysicsMapCacheManger.Cache(type);
        }
    }
}