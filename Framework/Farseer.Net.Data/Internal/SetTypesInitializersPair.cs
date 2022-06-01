using System;
using System.Collections.Generic;
using Collections.Pooled;

namespace FS.Data.Internal
{
    /// <summary>
    /// 保存一个DbContext对象下的所有Set实体类型
    /// </summary>
    internal class SetTypesInitializersPair : Tuple<IDictionary<Type, PooledList<string>>, Action<DbContext>>
    {
        /// <summary>
        /// 保存一个DbContext对象下的所有Set实体类型
        /// </summary>
        public SetTypesInitializersPair(IDictionary<Type, PooledList<string>> entityTypeToPropertyNameMap, Action<DbContext> setsInitializer) : base(item1: entityTypeToPropertyNameMap, item2: setsInitializer)
        {
        }

        /// <summary>
        /// 一个DbContext对象下的所有Set实体类型
        /// Key：Set类型
        /// Value：PropertyName：一个Set<Entity>被多个类属性调用
        /// </summary>
        public IDictionary<Type, PooledList<string>> SetTypeList => Item1;

        /// <summary>
        /// 实体化所有Set属性
        /// </summary>
        public Action<DbContext> SetsInitializer => Item2;
    }
}