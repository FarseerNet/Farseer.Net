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
        private static readonly object LockObject = new object();

        private PropertySetCacheManger(PropertyInfo key) : base(key)
        {
        }

        protected override Action<object, object> SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }

                //缓存中没有找到，新建一个构造函数的委托
                return (CacheList[Key] = ExpressionHelper.SetValue(Key));
            }
        }

        /// <summary>
        ///     赋值
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="instance">对象</param>
        /// <param name="value">要设置的值</param>
        public static void Cache(PropertyInfo key, object instance, object value)
        {
            new PropertySetCacheManger(key).GetValue()(instance, value);
        }
    }
}