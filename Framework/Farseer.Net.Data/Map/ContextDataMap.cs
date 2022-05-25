using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FS.Data.Cache;
using FS.Data.Inteface;
using FS.Data.Internal;

namespace FS.Data.Map
{
    /// <summary>
    ///     上下文数据映射
    /// </summary>
    internal class ContextDataMap
    {
        /// <summary>
        ///     物理结构
        /// </summary>
        public readonly ContextPhysicsMap ContextPhysicsMap;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type"> 实体类Type </param>
        /// <param name="context"> 上下文 </param>
        internal ContextDataMap(Type type, InternalContext context)
        {
            // 映射所有Set属性的结构
            ContextPhysicsMap = ContextMapCacheManger.Cache(key: type);
            var dbName = context.DbProvider.ConnectionString.GetDbName(context.DatabaseConnection.ConnectionString);
            // 初始化Set属性的数据
            foreach (var setPhysicsMap in ContextPhysicsMap.EntityMapList)
            {
                setPhysicsMap.Value.InternalContext ??= context;
                SetDataList.Add(item: new SetDataMap(entityPhysicsMap: setPhysicsMap, dbName));
            }
        }

        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public List<SetDataMap> SetDataList { get; } = new();

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="setPropertyInfo"> 属性变量 </param>
        /// <returns> </returns>
        public SetDataMap GetEntityMap(PropertyInfo setPropertyInfo) => SetDataList.FirstOrDefault(predicate: o => o.ClassProperty == setPropertyInfo);

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="setType"> 属性变量 </param>
        /// <param name="propertyName"> 属性名称 </param>
        public SetDataMap GetEntityMap(Type setType, string propertyName) => SetDataList.FirstOrDefault(predicate: o => o.ClassProperty.PropertyType == setType && o.ClassProperty.Name == propertyName);
    }
}