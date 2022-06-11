﻿using System;
using System.Reflection;
using FS.Utils.Common;

namespace FS.Cache
{
    /// <summary>
    ///     表达式树委托Get缓存
    /// </summary>
    public class StaticPropertyGetCacheManger : AbsCacheManger<PropertyInfo, Func<object, object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private StaticPropertyGetCacheManger(PropertyInfo key) : base(key: key)
        {
        }

        /// <summary>
        ///     当缓存不存在时，上锁加入缓存
        /// </summary>
        protected override Func<object, object> SetCacheLock()
        {
            if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
            lock (LockObject)
            {
                if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
                //缓存中没有找到，新建一个构造函数的委托
                return CacheList[key: Key] = ExpressionHelper.GetStaticValue(Key);
            }
        }

        /// <summary>
        ///     赋值
        /// </summary>
        /// <param name="key"> 缓存Key </param>
        public static object Cache(PropertyInfo key) => new StaticPropertyGetCacheManger(key: key).GetValue()(arg: null);
    }
}