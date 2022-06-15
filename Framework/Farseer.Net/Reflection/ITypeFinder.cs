using System;
using System.Collections.Generic;
using System.Reflection;
using Collections.Pooled;

namespace FS.Reflection
{
    /// <summary>
    ///     类型查找器
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        ///     根据条件查找类型
        /// </summary>
        PooledList<Type> Find(Func<Type, bool> predicate);

        /// <summary>
        ///     查找所有的类型
        /// </summary>
        IEnumerable<Type> FindAll();

        /// <summary>
        ///     找继承TType接口的实现类
        /// </summary>
        PooledList<Type> Find<TInterface>();

        /// <summary>
        /// 忽略微软及常用的程序集
        /// </summary>
        PooledList<Assembly> IgnoreAssembly(IEnumerable<Assembly> assemblies);
        /// <summary>
        ///     找到类中使用了指定特性的类Type
        /// </summary>
        PooledList<Type> FindAttribute<TAttribute>() where TAttribute : Attribute;
    }
}