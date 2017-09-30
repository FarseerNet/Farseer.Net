using System;

namespace Farseer.Net.Cache.TypeManger
{
    /// <summary>
    ///     表达式树委托实例化缓存
    /// </summary>
    public class InstanceCacheManger : AbsCacheManger<int, object>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private readonly Type _type;
        private readonly object[] _parameterTypes;

        private InstanceCacheManger(Type type, params object[] args)
        {
            _type = type;
            _parameterTypes = args;
            // 缓存
            Key = type.GetHashCode();
            //根据参数列表返回参数类型数组
            if (args != null && args.Length > 0)
            {
                foreach (var arg in args) { Key += arg.GetHashCode(); }
            }
        }

        /// <inherit />
        protected override object SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }
                //缓存中没有找到，新建一个构造函数的委托
                return (CacheList[Key] = Activator.CreateInstance(_type, _parameterTypes));
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="args">对象构造参数</param>
        public static object Cache(Type type, params object[] args) => new InstanceCacheManger(type, args).GetValue();
    }
}