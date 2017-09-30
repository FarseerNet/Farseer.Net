using System;

namespace Farseer.Net.Modules
{
    /// <summary>
    ///     依赖其他模块的标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
        /// <summary>
        ///     依赖的模块类型
        /// </summary>
        public Type[] DependedModuleTypes { get; private set; }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="dependedModuleTypes">依赖的模块类型</param>
        public DependsOnAttribute(params Type[] dependedModuleTypes) { DependedModuleTypes = dependedModuleTypes; }
    }
}