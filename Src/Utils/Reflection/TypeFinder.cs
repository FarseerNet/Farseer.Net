using System;
using System.Reflection;
using System.Linq;

namespace FS.Reflection
{
    /// <summary>
    /// 类型查找器
    /// </summary>
    public struct TypeFinder : ITypeFinder
    {
        /// <summary>
        ///  被查找类型所在的指定程序集
        /// </summary>
        public Assembly[] Assemblys { get; private set; }
        public TypeFinder(params Assembly[] assemblys)
        {
            if (assemblys == null) throw new ArgumentNullException(nameof(assemblys));

            Assemblys = assemblys;
        }
        /// <summary>
        /// 查找指定条件的类型
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns></returns>
        public Type[] Find(Func<Type, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return FindAll().Where(predicate).ToArray();
        }

        /// <summary>
        /// 查找指定程序集中的所有类型
        /// </summary>
        /// <returns></returns>
        public Type[] FindAll()
        {
            return Assemblys.SelectMany(
                assembly => assembly.GetTypes(),
                (assembly, type) => type).
                Distinct().ToArray();
        }
    }
}
