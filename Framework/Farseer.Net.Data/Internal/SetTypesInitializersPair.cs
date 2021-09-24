using System;
using System.Collections.Generic;

namespace FS.Data.Internal
{
    // <summary>
    // 保存一个DbContext对象下的所有Set实体类型
    // </summary>
    internal class SetTypesInitializersPair : Tuple<Dictionary<Type, List<string>>, Action<DbContext>>
    {
        // <summary>
        // 保存一个DbContext对象下的所有Set实体类型
        // </summary>
        public SetTypesInitializersPair(Dictionary<Type, List<string>> entityTypeToPropertyNameMap, Action<DbContext> setsInitializer) : base(item1: entityTypeToPropertyNameMap, item2: setsInitializer)
        {
        }

        // <summary>
        // 一个DbContext对象下的所有Set实体类型
        // Key：Set类型
        // Value：PropertyName：一个Set<Entity>被多个类属性调用
        // </summary>
        public Dictionary<Type, List<string>> SetTypeList => Item1;

        // <summary>
        // 实体化所有Set属性
        // </summary>
        public Action<DbContext> SetsInitializer => Item2;
    }
}