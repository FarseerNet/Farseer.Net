using System;
using System.Reflection;
using Farseer.Net.Utils.Common;

namespace Farseer.Net.Cache
{
    /// <summary>
    ///     表达式树委托Get缓存
    /// </summary>
    public class FieldGetCacheManger : AbsCacheManger<FieldInfo, Func<object, object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private FieldGetCacheManger(FieldInfo key) : base(key)
        {
        }

        /// <summary>
        ///     当缓存不存在时，上锁加入缓存
        /// </summary>
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
        /// 赋值
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="instance">对象</param>
        public static object Cache(FieldInfo key, object instance)
        {
            return new FieldGetCacheManger(key).GetValue()(instance);
        }
    }
}