using System.Collections.Generic;
using System.Reflection;
using Farseer.Net.Core.Mapping;
using Farseer.Net.Data.Cache;
using Farseer.Net.Data.Features;

namespace Farseer.Net.Data.Map
{
    /// <summary>
    ///     实体类结构映射
    /// </summary>
    public class SetDataMap
    {
        internal SetDataMap(KeyValuePair<PropertyInfo, SetPhysicsMap> entityPhysicsMap)
        {
            ClassProperty = entityPhysicsMap.Key;
            PhysicsMap = entityPhysicsMap.Value;
            Name = ClassProperty.Name;
        }

        /// <summary>
        ///     物理结构
        /// </summary>
        public SetPhysicsMap PhysicsMap { get; private set; }

        /// <summary>
        ///     表/视图/存储过程名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     类属性
        /// </summary>
        internal PropertyInfo ClassProperty { get; }

        /// <summary>
        ///     逻辑删除
        /// </summary>
        internal SortDelete SortDelete { get; private set; }

        /// <summary>
        ///     设置表/视图/存储过程名称
        /// </summary>
        /// <param name="name">表/视图/存储过程名称</param>
        public SetDataMap SetName(string name)
        {
            Name = name;
            return this;
        }

        ///// <summary>
        ///// 设置外键关系
        ///// </summary>
        ///// <param name="propertyName">实体类属性名称</param>
        ///// <param name="foreignSet">外键Set属性名称</param>
        ///// <param name="foreignProperty">外键字段属性名称</param>
        //public EntityDataMap SetForeignKey(string propertyName, string foreignSet, string foreignProperty)
        //{
        //    return this;
        //}

        /// <summary>
        ///     设置逻辑删除方案
        /// </summary>
        /// <param name="name">软删除标记字段名称</param>
        /// <param name="sortDeleteType">数据库字段类型</param>
        /// <param name="value">标记值</param>
        public SetDataMap SetSortDelete(string name, eumSortDeleteType sortDeleteType, object value)
        {
            Check.NotEmpty(name, "字段名称不能为空");
            Check.IsTure(sortDeleteType != eumSortDeleteType.DateTime && value == null, "非时间类型时，value不能为空");

            SortDelete = SortDeleteCacheManger.Cache(name, sortDeleteType, value, ClassProperty.PropertyType.GetGenericArguments()[0]);
            return this;
        }
    }
}