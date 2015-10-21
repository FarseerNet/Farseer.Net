using System;
using FS.Utils.Common;

namespace FS.Cache
{
    /// <summary>
    ///     表达式树委托实例化缓存
    /// </summary>
    public class InstanceCacheManger : AbsCacheManger<int, Func<object[], object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private readonly Type _type;
        private readonly Type[] _parameterTypes;

        private InstanceCacheManger(Type type, params object[] args)
        {
            _type = type;
            // 缓存
            Key = type.GetHashCode();
            //根据参数列表返回参数类型数组
            _parameterTypes = null;
            if (args != null && args.Length > 0)
            {
                _parameterTypes = new Type[args.Length];
                for (var i = 0; i < args.Length; i++)
                {
                    _parameterTypes[i] = args[i].GetType();
                    Key += _parameterTypes[i].GetHashCode();
                }
            }
        }

        protected override Func<object[], object> SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }
                //缓存中没有找到，新建一个构造函数的委托
                return (CacheList[Key] = ExpressionHelper.CreateInstance(_type, _parameterTypes));
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="args">对象构造参数</param>
        public static object Cache(Type type, params object[] args)
        {
            return new InstanceCacheManger(type, args).GetValue()(args);
        }
    }
}