using System;
using System.Collections.Generic;
using System.Reflection;

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
        /// <param name="predicate"> </param>
        /// <returns> </returns>
        Type[] Find(Func<Type, bool> predicate);

        /// <summary>
        ///     查找所有的类型
        /// </summary>
        /// <returns> </returns>
        Type[] FindAll();
        /// <summary>
        ///     找继承TType接口的实现类
        /// </summary>
        Type[] Find<TInterface>();
        /// <summary>
        /// 忽略微软及常用的程序集
        /// </summary>
        IEnumerable<Assembly> IgnoreAssembly(List<Assembly> assemblies);
    }
}