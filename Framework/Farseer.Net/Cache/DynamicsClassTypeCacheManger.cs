using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FS.Utils.Common;

namespace FS.Cache
{
    /// <summary>
    ///     创建动态类缓存
    /// </summary>
    public class DynamicsClassTypeCacheManger : AbsCacheManger<int, Type>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private readonly Type   _baseType;
        private readonly Type[] _constructors;

        private readonly Dictionary<string, Type> _dicAddPropertys;
        private readonly bool                     _isListProperty;
        private readonly List<PropertyInfo>       _lstAddPropertys;

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="addPropertys"> Key：属性名称；Value：属性类型 </param>
        /// <param name="baseType"> 继承的父类类型 </param>
        /// <param name="constructors"> 构造函数参数 </param>
        private DynamicsClassTypeCacheManger(List<PropertyInfo> addPropertys, Type[] constructors, Type baseType) : base(key: 0)
        {
            _lstAddPropertys = addPropertys;
            _constructors    = constructors;
            _baseType        = baseType;
            _isListProperty  = true;

            if (addPropertys  != null) Key += addPropertys.Sum(selector: propertyInfo => propertyInfo.GetHashCode());
            if (_baseType     != null) Key += _baseType.GetHashCode();
            if (_constructors != null) Key += _constructors.Sum(selector: constructor => constructor.GetHashCode());
        }

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="addPropertys"> Key：属性名称；Value：属性类型 </param>
        /// <param name="baseType"> 继承的父类类型 </param>
        private DynamicsClassTypeCacheManger(Dictionary<string, Type> addPropertys, Type baseType = null) : base(key: 0)
        {
            _dicAddPropertys = addPropertys;
            _baseType        = baseType;
            _isListProperty  = false;

            Check.IsTure(isTrue: addPropertys == null, parameterName: "propertys参数不能为空或为0");

            if (addPropertys != null) Key += addPropertys.Sum(selector: propertyInfo => propertyInfo.Value.GetHashCode());
            if (baseType     != null) Key += baseType.GetHashCode();
        }

        /// <summary>
        ///     当缓存不存在时，上锁加入缓存
        /// </summary>
        protected override Type SetCacheLock()
        {
            if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
            lock (LockObject)
            {
                if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
                //缓存中没有找到，新建一个构造函数的委托
                var val = _isListProperty ? Dynamics.CreateClassType(addPropertys: _lstAddPropertys, constructors: _constructors, baseType: _baseType) : Dynamics.CreateClassType(addPropertys: _dicAddPropertys, baseType: _baseType);
                return CacheList[key: Key] = val;
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="addPropertys"> Key：属性名称；Value：属性类型 </param>
        /// <param name="baseType"> 继承的父类类型 </param>
        /// <param name="constructors"> 构造函数参数 </param>
        public static Type Cache(List<PropertyInfo> addPropertys, Type[] constructors = null, Type baseType = null) => new DynamicsClassTypeCacheManger(addPropertys: addPropertys, constructors: constructors, baseType: baseType).GetValue();

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="addPropertys"> Key：属性名称；Value：属性类型 </param>
        /// <param name="baseType"> 继承的父类类型 </param>
        public static Type Cache(Dictionary<string, Type> addPropertys, Type baseType = null) => new DynamicsClassTypeCacheManger(addPropertys: addPropertys, baseType: baseType).GetValue();
    }
}