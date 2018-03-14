using System;
using System.Collections.Generic;
using System.Reflection;
using FS.Data.Cache;

namespace FS.Data.Map
{
    /// <summary>
    ///     上下文结构映射
    /// </summary>
    public class ContextPhysicsMap
    {
        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public ContextPhysicsMap(Type type)
        {
            Type = type;

            // 当前调用的程序集，用于判断是否属于Farseer.Net.dll程序集
            var currentAssembly = Assembly.GetCallingAssembly();
            // 遍历所有Set属性(表、视图、存储过程的名称),取得对应使用标记名称
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.Public))
            {
                // 必须是Farseer.Net.dll程序集
                if (!propertyInfo.CanWrite || propertyInfo.PropertyType.Assembly != currentAssembly || (propertyInfo.DeclaringType != null && propertyInfo.DeclaringType.Assembly == currentAssembly)) { continue; }

                // 设置每个Set属性（目前有Set是非泛型的）
                EntityMapList[propertyInfo] = propertyInfo.PropertyType.IsGenericType ? SetMapCacheManger.Cache(propertyInfo.PropertyType.GetGenericArguments()[0]) : null;
            }
        }

        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public Dictionary<PropertyInfo, SetPhysicsMap> EntityMapList { get; } = new Dictionary<PropertyInfo, SetPhysicsMap>();

        /// <summary>
        ///     类型
        /// </summary>
        private Type Type { get; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator ContextPhysicsMap(Type type) => ContextMapCacheManger.Cache(type);
    }
}