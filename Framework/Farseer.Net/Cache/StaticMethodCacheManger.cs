﻿using System;
using System.Reflection;
using Collections.Pooled;

namespace FS.Cache
{
    /// <summary>
    ///     调用静态方法
    /// Func<object[], object> ,Key:方法的参数，Value：方法的返回值
    /// </summary>
    public class StaticMethodCacheManger : AbsCacheManger<int, Func<object[], object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private readonly string _methodName;

        private readonly Type _type;

        private StaticMethodCacheManger(Type type, string methodName, params object[] args)
        {
            _type       = type;
            _methodName = methodName;

            // 缓存
            Key = type.GetHashCode() + methodName.GetHashCode();
            //根据参数列表返回参数类型数组
            if (args is { Length: > 0 })
            {
                foreach (var t in args)
                {
                    Key += t.GetType().GetHashCode();
                }
            }
        }

        /// <summary>
        ///     当缓存不存在时，上锁加入缓存
        /// </summary>
        protected override Func<object[], object> SetCacheLock()
        {
            if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
            lock (LockObject)
            {
                if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
                var method = _type.GetMethod(name: _methodName, bindingAttr: BindingFlags.Static | BindingFlags.Public);
                //缓存中没有找到，新建一个构造函数的委托
                return CacheList[key: Key] = args => method.Invoke(obj: null, parameters: args);
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="type"> 对象类型 </param>
        /// <param name="methodName"> 方法名称 </param>
        /// <param name="args"> 对象构造参数 </param>
        public static object Cache(Type type, string methodName, params object[] args) => new StaticMethodCacheManger(type: type, methodName: methodName, args: args).GetValue()(arg: args);
    }
}