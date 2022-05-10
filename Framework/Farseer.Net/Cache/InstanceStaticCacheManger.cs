using System;
using System.Reflection;

namespace FS.Cache
{
    /// <summary>
    ///     根据对象Type，创建实例（替换反射创建实例）
    /// </summary>
    public class InstanceStaticCacheManger : AbsCacheManger<int, Func<object[], object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private readonly string _methodName;

        private readonly Type _type;

        private InstanceStaticCacheManger(Type type, string methodName, params object[] args)
        {
            _type       = type;
            _methodName = methodName;

            // 缓存
            Key = type.GetHashCode() + methodName.GetHashCode();
            //根据参数列表返回参数类型数组
            if (args != null && args.Length > 0)
            {
                var parameterTypes = new Type[args.Length];
                for (var i = 0; i < args.Length; i++)
                {
                    parameterTypes[i] =  args[i].GetType();
                    Key               += parameterTypes[i].GetHashCode();
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
                return CacheList[key: Key] = param => method.Invoke(obj: null, parameters: param);
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="type"> 对象类型 </param>
        /// <param name="methodName"> 方法名称 </param>
        /// <param name="args"> 对象构造参数 </param>
        public static object Cache(Type type, string methodName, params object[] args) => new InstanceStaticCacheManger(type: type, methodName: methodName, args: args).GetValue()(arg: args);
    }
}