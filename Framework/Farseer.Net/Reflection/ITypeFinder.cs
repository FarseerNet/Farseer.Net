
using System;

namespace Farseer.Net.Reflection
{
    /// <summary>
    /// 类型查找器
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// 根据条件查找类型
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Type[] Find(Func<Type, bool> predicate);

        /// <summary>
        /// 查找所有的类型
        /// </summary>
        /// <returns></returns>
        Type[] FindAll();
    }
}