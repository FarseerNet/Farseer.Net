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
        private static readonly object LockObject = new object();

        private readonly Dictionary<string, Type> _dicAddPropertys;
        private readonly List<PropertyInfo> _lstAddPropertys;
        private readonly Type[] _constructors;
        private readonly Type _baseType;
        private readonly bool _isListProperty;

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="addPropertys">Key：属性名称；Value：属性类型</param>
        /// <param name="baseType">继承的父类类型</param>
        /// <param name="constructors">构造函数参数</param>
        private DynamicsClassTypeCacheManger(List<PropertyInfo> addPropertys, Type[] constructors, Type baseType) : base(0)
        {
            this._lstAddPropertys = addPropertys;
            this._constructors = constructors;
            this._baseType = baseType;
            this._isListProperty = true;

            if (addPropertys != null) { Key += addPropertys.Sum(propertyInfo => propertyInfo.GetHashCode()); }
            if (_baseType != null) { Key += _baseType.GetHashCode(); }
            if (_constructors != null) { Key += _constructors.Sum(constructor => constructor.GetHashCode()); }
        }

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="addPropertys">Key：属性名称；Value：属性类型</param>
        /// <param name="baseType">继承的父类类型</param>
        private DynamicsClassTypeCacheManger(Dictionary<string, Type> addPropertys, Type baseType = null) : base(0)
        {
            this._dicAddPropertys = addPropertys;
            this._baseType = baseType;
            this._isListProperty = false;

            Check.IsTure(addPropertys == null, "propertys参数不能为空或为0");

            if (addPropertys != null) { Key += addPropertys.Sum(propertyInfo => propertyInfo.Value.GetHashCode()); }
            if (baseType != null) { Key += baseType.GetHashCode(); }
        }

        protected override Type SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }

                //缓存中没有找到，新建一个构造函数的委托
                var val = _isListProperty ? Dynamics.CreateClassType(_lstAddPropertys, _constructors, _baseType) : Dynamics.CreateClassType(_dicAddPropertys, _baseType);
                return (CacheList[Key] = val);
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="addPropertys">Key：属性名称；Value：属性类型</param>
        /// <param name="baseType">继承的父类类型</param>
        /// <param name="constructors">构造函数参数</param>
        public static Type Cache(List<PropertyInfo> addPropertys, Type[] constructors = null, Type baseType = null)
        {
            return new DynamicsClassTypeCacheManger(addPropertys, constructors, baseType).GetValue();
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="addPropertys">Key：属性名称；Value：属性类型</param>
        /// <param name="baseType">继承的父类类型</param>
        public static Type Cache(Dictionary<string, Type> addPropertys, Type baseType = null)
        {
            return new DynamicsClassTypeCacheManger(addPropertys, baseType).GetValue();
        }
    }
}