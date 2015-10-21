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

        private readonly Dictionary<string, Type> _dicPropertys;
        private readonly List<PropertyInfo> _lstPropertys;
        private readonly Type[] _constructors = null;
        private readonly Type _parentType = null;
        private readonly bool _isListProperty;

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="propertys">Key：属性名称；Value：属性类型</param>
        /// <param name="parentType">继承的父类类型</param>
        /// <param name="constructors">构造函数参数</param>
        private DynamicsClassTypeCacheManger(List<PropertyInfo> propertys, Type[] constructors, Type parentType) : base(0)
        {
            this._lstPropertys = propertys;
            this._constructors = constructors;
            this._parentType = parentType;
            this._isListProperty = true;

            if (propertys != null) { Key += propertys.Sum(propertyInfo => propertyInfo.GetHashCode()); }
            if (_parentType != null) { Key += _parentType.GetHashCode(); }
            if (_constructors != null) { Key += _constructors.Sum(constructor => constructor.GetHashCode()); }
        }

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="propertys">Key：属性名称；Value：属性类型</param>
        /// <param name="parentType">继承的父类类型</param>
        private DynamicsClassTypeCacheManger(Dictionary<string, Type> propertys, Type parentType = null) : base(0)
        {
            this._dicPropertys = propertys;
            this._parentType = parentType;
            this._isListProperty = false;

            Check.IsTure(propertys == null, "propertys参数不能为空或为0");

            if (propertys != null) { Key += propertys.Sum(propertyInfo => propertyInfo.Value.GetHashCode()); }
            if (parentType != null) { Key += parentType.GetHashCode(); }
        }

        protected override Type SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }

                //缓存中没有找到，新建一个构造函数的委托
                var val = _isListProperty ? Dynamics.CreateClassType(_lstPropertys, _constructors, _parentType) : Dynamics.CreateClassType(_dicPropertys, _parentType);
                return (CacheList[Key] = val);
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="propertys">Key：属性名称；Value：属性类型</param>
        /// <param name="parentType">继承的父类类型</param>
        /// <param name="constructors">构造函数参数</param>
        public static Type Cache(List<PropertyInfo> propertys, Type[] constructors = null, Type parentType = null)
        {
            return new DynamicsClassTypeCacheManger(propertys, constructors, parentType).GetValue();
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="propertys">Key：属性名称；Value：属性类型</param>
        /// <param name="parentType">继承的父类类型</param>
        public static Type Cache(Dictionary<string, Type> propertys, Type parentType = null)
        {
            return new DynamicsClassTypeCacheManger(propertys, parentType).GetValue();
        }
    }
}