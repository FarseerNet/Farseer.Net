using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FS.ElasticSearch.Cache;

namespace FS.ElasticSearch.Map
{
    /// <summary>
    ///     上下文数据映射
    /// </summary>
    public class ContextDataMap
    {
        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public ContextDataMap(Type type)
        {
            // 映射所有Set属性的结构
            PhysicsMap = ContextMapCacheManger.Cache(type);
            
            // 初始化Set属性的数据
            foreach (var setPhysicsMap in PhysicsMap.EntityMapList) { SetDataList.Add(new SetDataMap(setPhysicsMap)); }
        }

        /// <summary>
        ///     物理结构
        /// </summary>
        public readonly ContextPhysicsMap PhysicsMap;

        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public List<SetDataMap> SetDataList { get; } = new();

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="setPropertyInfo">属性变量</param>
        public SetDataMap GetEntityMap(PropertyInfo setPropertyInfo) => SetDataList.FirstOrDefault(o => o.ClassProperty == setPropertyInfo);

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="setType">属性变量</param>
        /// <param name="propertyName">属性名称</param>
        public SetDataMap GetEntityMap(Type setType, string propertyName) => SetDataList.FirstOrDefault(o => o.ClassProperty.PropertyType == setType && o.ClassProperty.Name == propertyName);
    }
}