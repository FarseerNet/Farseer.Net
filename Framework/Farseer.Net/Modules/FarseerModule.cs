using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Farseer.Net.Configuration.Startup;
using Farseer.Net.DI;

namespace Farseer.Net.Modules
{
    /// <summary>
    ///     模块基类
    ///     模块系统的基类，所有模块都需要继承自此类，来完成依赖注入和模块初始化操作
    /// </summary>
    public abstract class FarseerModule
    {
        /// <summary>
        ///     依赖注入管理器
        /// </summary>
        protected internal IIocManager IocManager { get; internal set; }

        /// <summary>
        ///     初始配置
        /// </summary>
        protected internal IFarseerStartupConfiguration Configuration { get; internal set; }

        /// <summary>
        ///     日志
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        ///     构造函数
        /// </summary>
        protected FarseerModule() { Logger = NullLogger.Instance; }

        /// <summary>
        ///     预初始化
        /// </summary>
        public virtual void PreInitialize() { }

        /// <summary>
        ///     初始化
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        ///     初始化之后
        /// </summary>
        public virtual void PostInitialize() { }

        /// <summary>
        ///     应用关闭之前先关闭模块
        /// </summary>
        public virtual void Shutdown() { }

        /// <summary>
        ///     获取模块附加的程序集
        /// </summary>
        /// <returns></returns>
        public virtual Assembly[] GetAdditionalAssemblies() => new Assembly[0];

        /// <summary>
        ///     给定类型是否FarseerModule
        /// </summary>
        /// <param name="type">Type to check</param>
        public static bool IsFarseerModule(Type type) => type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsGenericType && typeof(FarseerModule).IsAssignableFrom(type);

        /// <summary>
        ///     查找模块依赖的模块
        /// </summary>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsFarseerModule(moduleType)) throw new FarseerInitException("此类型不是一个有效的模块: " + moduleType.AssemblyQualifiedName);

            var list = new List<Type>();

            if (moduleType.GetTypeInfo().IsDefined(typeof(DependsOnAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetTypeInfo().GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes) foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes) list.Add(dependedModuleType);
            }

            return list;
        }

        /// <summary>
        ///     递归获取所有依赖模块
        /// </summary>
        public static List<Type> FindDependedModuleTypesRecursively(Type moduleType)
        {
            var list = new List<Type>();
            AddModuleAndDependenciesResursively(list, moduleType);
            if (!list.Contains(typeof(FarseerKernelModule))) { list.Add(typeof(FarseerKernelModule));}
            return list;
        }

        /// <summary>
        ///     递归魔惑所有依赖模块
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="module"></param>
        private static void AddModuleAndDependenciesResursively(List<Type> modules, Type module)
        {
            if (!IsFarseerModule(module)) throw new FarseerInitException("此类型不是一个有效的模块: " + module.AssemblyQualifiedName);

            if (modules.Contains(module)) return;

            modules.Add(module);

            var dependedModules = FindDependedModuleTypes(module);
            foreach (var dependedModule in dependedModules) AddModuleAndDependenciesResursively(modules, dependedModule);
        }
    }
}