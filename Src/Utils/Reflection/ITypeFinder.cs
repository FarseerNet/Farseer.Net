using System;
using System.Reflection;

namespace FS.Reflection
{
    /// <summary>
    /// 类型查找器
    /// </summary>
    public interface ITypeFinder : IFinder<Type>
    {
        /// <summary>
        /// 被查找类型所在的指定程序集
        /// </summary>
         Assembly[] Assemblys { get; }
    }
}
