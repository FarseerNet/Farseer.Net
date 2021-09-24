using System;
using System.Reflection;
using FS.Utils.Common;

namespace FS.Cache
{
    /// <summary>
    ///     表达式树委托Set缓存
    /// </summary>
    public class PropertySetCacheManger : AbsCacheManger<PropertyInfo, Action<object, object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private PropertySetCacheManger(PropertyInfo key) : base(key: key)
        {
        }

        /// <summary>
        ///     当缓存不存在时，上锁加入缓存
        /// </summary>
        protected override Action<object, object> SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];

                //缓存中没有找到，新建一个构造函数的委托
                return CacheList[key: Key] = ExpressionHelper.SetValue(propertyInfo: Key);
            }
        }

        /// <summary>
        ///     赋值
        /// </summary>
        /// <param name="key"> 缓存Key </param>
        /// <param name="instance"> 对象 </param>
        /// <param name="value"> 要设置的值 </param>
        public static void Cache(PropertyInfo key, object instance, object value)
        {
            new PropertySetCacheManger(key: key).GetValue()(arg1: instance, arg2: value);
        }
    }
}