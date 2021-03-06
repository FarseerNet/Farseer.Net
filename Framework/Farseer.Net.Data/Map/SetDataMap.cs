﻿using System.Collections.Generic;
using System.Reflection;
using FS.Data.Cache;
using FS.Data.Features;

namespace FS.Data.Map
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
            TableName = ClassProperty.Name;
        }

        /// <summary>
        ///     物理结构
        /// </summary>
        public SetPhysicsMap PhysicsMap { get; private set; }

        /// <summary>
        ///     库名称
        /// </summary>
        public string DbName { get; private set; }

        /// <summary>
        ///     表/视图/存储过程名称
        /// </summary>
        public string TableName { get; private set; }

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
        /// <param name="tableName">表/视图/存储过程名称</param>
        public SetDataMap SetName(string tableName)
        {
            TableName = tableName;
            return this;
        }

        /// <summary>
        ///     设置表/视图/存储过程名称
        /// </summary>
        /// <param name="dbName">库名称 </param>
        /// <param name="tableName">表/视图/存储过程名称</param>
        public SetDataMap SetName(string dbName, string tableName)
        {
            DbName = dbName;
            TableName = tableName;
            return this;
        }

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