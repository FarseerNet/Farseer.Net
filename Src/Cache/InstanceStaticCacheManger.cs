using System;
using System.Reflection;
using FS.Utils.Common;

namespace FS.Cache
{
    /// <summary>
    ///     表达式树委托实例化缓存
    /// </summary>
    public class InstanceStaticCacheManger : AbsCacheManger<int, Func<object[], object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private readonly Type _type;
        private readonly string _methodName;

        private InstanceStaticCacheManger(Type type, string methodName, params object[] args)
        {
            _type = type;
            _methodName = methodName;

            // 缓存
            Key = type.GetHashCode() + methodName.GetHashCode();
            //根据参数列表返回参数类型数组
            if (args != null && args.Length > 0)
            {
                var parameterTypes = new Type[args.Length];
                for (var i = 0; i < args.Length; i++)
                {
                    parameterTypes[i] = args[i].GetType();
                    Key += parameterTypes[i].GetHashCode();
                }
            }
        }

        protected override Func<object[], object> SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }
                var method = _type.GetMethod(_methodName, BindingFlags.Static | BindingFlags.Public);
                //缓存中没有找到，新建一个构造函数的委托
                return (CacheList[Key] = param => method.Invoke(null, param));
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">对象构造参数</param>
        public static object Cache(Type type, string methodName, params object[] args)
        {
            return new InstanceStaticCacheManger(type, methodName, args).GetValue()(args);
        }
    }
}