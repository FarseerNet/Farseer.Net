using System;
using System.Collections.Generic;

namespace FS.ElasticSearch.Internal
{
    // <summary>
    // 保存一个EsContext对象下的所有Set实体类型
    // </summary>
    internal class SetTypesInitializersPair : Tuple<Dictionary<Type, List<string>>, Action<EsContext>>
    {
        // <summary>
        // 保存一个EsContext对象下的所有Set实体类型
        // </summary>
        public SetTypesInitializersPair(Dictionary<Type, List<string>> entityTypeToPropertyNameMap, Action<EsContext> setsInitializer) : base(entityTypeToPropertyNameMap, setsInitializer)
        {
        }

        // <summary>
        // 一个EsContext对象下的所有Set实体类型
        // Key：Set类型
        // Value：PropertyName：一个Set<Entity>被多个类属性调用
        // </summary>
        public Dictionary<Type, List<string>> SetTypeList => Item1;

        // <summary>
        // 实体化所有Set属性
        // </summary>
        public Action<EsContext> SetsInitializer => Item2;
    }
}