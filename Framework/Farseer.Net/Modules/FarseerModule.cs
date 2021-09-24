using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FS.Configuration.Startup;
using FS.DI;

namespace FS.Modules
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
        ///     预初始化
        /// </summary>
        public virtual void PreInitialize()
        {
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        ///     初始化之后
        /// </summary>
        public virtual void PostInitialize()
        {
        }

        /// <summary>
        ///     应用关闭之前先关闭模块
        /// </summary>
        public virtual void Shutdown()
        {
        }

        /// <summary>
        ///     获取模块附加的程序集
        /// </summary>
        /// <returns> </returns>
        public virtual Assembly[] GetAdditionalAssemblies() => new Assembly[0];

        /// <summary>
        ///     给定类型是否FarseerModule
        /// </summary>
        /// <param name="type"> Type to check </param>
        public static bool IsFarseerModule(Type type) => type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsGenericType && typeof(FarseerModule).IsAssignableFrom(c: type);

        /// <summary>
        ///     查找模块依赖的模块
        /// </summary>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsFarseerModule(type: moduleType)) throw new FarseerInitException(message: "此类型不是一个有效的模块: " + moduleType.AssemblyQualifiedName);

            var list = new List<Type>();

            if (moduleType.GetTypeInfo().IsDefined(attributeType: typeof(DependsOnAttribute), inherit: true))
            {
                var dependsOnAttributes = moduleType.GetTypeInfo().GetCustomAttributes(attributeType: typeof(DependsOnAttribute), inherit: true).Cast<DependsOnAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                        list.Add(item: dependedModuleType);
            }

            return list;
        }

        /// <summary>
        ///     递归获取所有依赖模块
        /// </summary>
        public static List<Type> FindDependedModuleTypesRecursively(Type moduleType)
        {
            var list = new List<Type>();
            AddModuleAndDependenciesResursively(modules: list, module: moduleType);
            if (!list.Contains(item: typeof(FarseerKernelModule))) list.Add(item: typeof(FarseerKernelModule));
            return list;
        }

        /// <summary>
        ///     递归魔惑所有依赖模块
        /// </summary>
        /// <param name="modules"> </param>
        /// <param name="module"> </param>
        private static void AddModuleAndDependenciesResursively(List<Type> modules, Type module)
        {
            if (!IsFarseerModule(type: module)) throw new FarseerInitException(message: "此类型不是一个有效的模块: " + module.AssemblyQualifiedName);

            if (modules.Contains(item: module)) return;

            modules.Add(item: module);

            var dependedModules = FindDependedModuleTypes(moduleType: module);
            foreach (var dependedModule in dependedModules) AddModuleAndDependenciesResursively(modules: modules, module: dependedModule);
        }
    }
}