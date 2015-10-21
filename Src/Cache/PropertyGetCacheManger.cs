using System;
using System.Reflection;
using FS.Utils.Common;

namespace FS.Cache
{
    /// <summary>
    ///     表达式树委托Get缓存
    /// </summary>
    public class PropertyGetCacheManger : AbsCacheManger<PropertyInfo, Func<object, object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private PropertyGetCacheManger(PropertyInfo key) : base(key)
        {
        }

        protected override Func<object, object> SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }

                //缓存中没有找到，新建一个构造函数的委托
                return (CacheList[Key] = ExpressionHelper.GetValue(Key));
            }
        }

        /// <summary>
        ///     赋值
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="instance">对象</param>
        public static object Cache(PropertyInfo key, object instance)
        {
            return new PropertyGetCacheManger(key).GetValue()(instance);
        }
    }
}