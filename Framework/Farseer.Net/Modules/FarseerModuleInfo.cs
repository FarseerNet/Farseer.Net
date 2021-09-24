using System;
using System.Collections.Generic;
using System.Reflection;

namespace FS.Modules
{
    /// <summary>
    ///     模块信息类
    /// </summary>
    public class FarseerModuleInfo
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="type"> 模块类型 </param>
        /// <param name="instance"> 模块实例 </param>
        public FarseerModuleInfo(Type type, FarseerModule instance)
        {
            Check.NotNull(value: type, parameterName: type.ToString());
            Check.NotNull(value: instance, parameterName: instance.ToString());

            Type     = type;
            Instance = instance;
            Assembly = Type.GetTypeInfo().Assembly;

            Dependencies = new List<FarseerModuleInfo>();
        }

        /// <summary>
        ///     包含模块定义的程序集
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        ///     模块类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     模块实例
        /// </summary>
        public FarseerModule Instance { get; }

        /// <summary>
        ///     依赖的其他模块
        /// </summary>
        public List<FarseerModuleInfo> Dependencies { get; }

        /// <summary>
        ///     重写，获取Type名称
        /// </summary>
        /// <returns> </returns>
        public override string ToString() => Type.AssemblyQualifiedName ?? Type.FullName;
    }
}