using System;
using System.Collections.Generic;
using Collections.Pooled;

namespace FS.Modules
{
    /// <summary>
    ///     模块管理器接口
    /// </summary>
    public interface IFarseerModuleManager
    {
        /// <summary>
        ///     启动模块
        /// </summary>
        FarseerModuleInfo StartupModule { get; }

        /// <summary>
        ///     模块列表
        /// </summary>
        PooledList<FarseerModuleInfo> Modules { get; }

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="startupModule"> </param>
        void Initialize(Type startupModule);

        /// <summary>
        ///     启动模块
        /// </summary>
        void StartModules();

        /// <summary>
        ///     关闭模块
        /// </summary>
        void ShutdownModules();
    }
}