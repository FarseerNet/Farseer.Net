using System;
using System.Reflection;
using Farseer.Net.Utils.Common;

namespace Farseer.Net.Cache
{
    /// <summary>
    ///     表达式树委托Set缓存
    /// </summary>
    public class FieldSetCacheManger : AbsCacheManger<FieldInfo, Action<object, object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private FieldSetCacheManger(FieldInfo key) : base(key)
        {
        }

        /// <summary>
        ///     当缓存不存在时，上锁加入缓存
        /// </summary>
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
        public static void Cache(FieldInfo key, object instance, object value)
        {
            new FieldSetCacheManger(key).GetValue()(instance, value);
        }
    }
}