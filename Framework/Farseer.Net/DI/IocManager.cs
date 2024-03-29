﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Collections.Pooled;
using FS.Modules;
using FS.Reflection;
using Microsoft.Extensions.Logging;

namespace FS.DI
{
    /// <summary>
    ///     依赖注入管理器，用来执行依赖注入和获取的任务
    /// </summary>
    public class IocManager : IIocManager
    {
        private static readonly object   objLock = new();
        private static readonly string[] ignores = { "System.", "Castle.", "Farseer.Net.", "mscorlib", "netstandard" };

        /// <summary>
        ///     约定注册器列表（目前有：普用、MVC控制器）
        /// </summary>
        private readonly PooledList<IConventionalDependencyRegistrar> _conventionalRegistrars = new();

        /// <summary>
        ///     是否已注册过WindsorInstaller
        /// </summary>
        private readonly PooledDictionary<Assembly, IWindsorInstaller> _isRegistrarWindsorInstaller = new();

        /// <summary>
        ///     构造函数
        /// </summary>
        static IocManager()
        {
            Instance = new IocManager();
            // 注册自己
            Instance.RegisterSelf();
        }

        /// <summary>
        ///     当前实例
        /// </summary>
        public static IocManager Instance { get; }

        /// <summary>
        ///     日志接口,没注册时，用默认的
        /// </summary>
        public ILogger Logger<T>()
        {
            if (IsRegistered<ILogger<T>>()) return Resolve<ILogger<T>>();
            var logger = Resolve<ILoggerFactory>().CreateLogger<T>();
            lock (objLock)
            {
                if (IsRegistered<ILogger<T>>()) return Resolve<ILogger<T>>();
                Container.Register(Component.For<ILogger<T>>().Instance(instance: logger).LifestyleSingleton());
            }

            return logger;
        }

        /// <summary>
        ///     依赖注入容器
        /// </summary>
        public IWindsorContainer Container { get; } = new WindsorContainer();

        /// <summary>
        ///     添加约定注册器
        /// </summary>
        public void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar) => _conventionalRegistrars.Add(item: registrar);

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        public void RegisterAssemblyByConvention(Type type)
        {
            using var assemblyNames = type.Assembly.GetReferencedAssemblies().Select(Assembly.Load).ToPooledList();
            assemblyNames.Add(type.Assembly);

            using var ignoreAssembly = GetService<ITypeFinder>().IgnoreAssembly(assemblyNames);
            RegisterAssemblyByConvention(ignoreAssembly);
        }

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        public void RegisterAssemblyByConvention(IEnumerable<Assembly> assemblys)
        {
            foreach (var assembly in assemblys.Where(assembly => !_isRegistrarWindsorInstaller.ContainsKey(key: assembly)))
            {
                // 忽略程序集
                if (ignores.Any(predicate: ignore => assembly.ManifestModule.Name.StartsWith(value: ignore))) continue;

                try
                {
                    RegisterAssemblyByConvention(assembly: assembly, config: new ConventionalRegistrationConfig());
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        public void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config)
        {
            var context          = new ConventionalRegistrationContext(assembly: assembly, iocManager: this, config: config);
            var windsorInstaller = FromAssembly.Instance(assembly: assembly);
            foreach (var registerer in _conventionalRegistrars) registerer.RegisterAssembly(context: context);
            if (config.InstallInstallers && windsorInstaller != null) Container.Install(windsorInstaller);

            _isRegistrarWindsorInstaller.Add(key: assembly, value: windsorInstaller);
        }

        /// <summary>
        ///     注册
        /// </summary>
        public void Register<TType>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class => Container.Register(string.IsNullOrEmpty(value: name) ? ApplyLifestyle(registration: Component.For<TType>(), lifeStyle: lifeStyle) : ApplyLifestyle(registration: Component.For<TType>().Named(name: name), lifeStyle: lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        public void Register(Type type, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) => Container.Register(string.IsNullOrEmpty(value: name) ? ApplyLifestyle(registration: Component.For(serviceType: type), lifeStyle: lifeStyle) : ApplyLifestyle(registration: Component.For(serviceType: type).Named(name: name), lifeStyle: lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        public void Register<TType, TImpl>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class where TImpl : class, TType => Container.Register(string.IsNullOrEmpty(value: name) ? ApplyLifestyle(registration: Component.For<TType, TImpl>().ImplementedBy<TImpl>(), lifeStyle: lifeStyle) : ApplyLifestyle(registration: Component.For<TType, TImpl>().Named(name: name).ImplementedBy<TImpl>(), lifeStyle: lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        public void Register(Type type, Type impl, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) => Container.Register(string.IsNullOrEmpty(value: name) ? ApplyLifestyle(registration: Component.For(type, impl).ImplementedBy(type: impl), lifeStyle: lifeStyle) : ApplyLifestyle(registration: Component.For(type, impl).Named(name: name).ImplementedBy(type: impl), lifeStyle: lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="instance">实例 </param>
        /// <param name="lifeStyle">实例生命周期</param>
        public void Register<T>(T instance, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class => Container.Register(ApplyLifestyle(Component.For<T>().Instance(instance), lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        public bool IsRegistered(Type type) => Container.Kernel.HasComponent(service: type);

        /// <summary>
        ///     注册
        /// </summary>
        public bool IsRegistered(string name) => Container.Kernel.HasComponent(name: name);

        /// <summary>
        ///     是否注册
        /// </summary>
        public bool IsRegistered<TType>() => Container.Kernel.HasComponent(service: typeof(TType));

        /// <summary>
        ///     获取实例
        /// </summary>
        public T Resolve<T>(string name = "") => string.IsNullOrEmpty(value: name) ? Container.Resolve<T>() : Container.Resolve<T>(key: name);

        /// <summary>
        ///     获取实例
        /// </summary>
        public T Resolve<T>(Type type, string name = "") => string.IsNullOrEmpty(value: name) ? (T)Container.Resolve(service: type) : (T)Container.Resolve(key: name, service: type);

        /// <summary>
        ///     获取实例
        /// </summary>
        public object Resolve(Type type, string name = "") => string.IsNullOrEmpty(value: name) ? Container.Resolve(service: type) : Container.Resolve(key: name, service: type);

        /// <summary>
        ///     获取所有实例
        /// </summary>
        public T[] ResolveAll<T>() => Container.ResolveAll<T>();

        /// <summary>
        ///     获取所有实例
        /// </summary>
        public object[] ResolveAll(Type type) => Container.ResolveAll(service: type).Cast<object>().ToArray();

        /// <summary>
        ///     释放
        /// </summary>
        public void Release(object obj) => Container.Release(instance: obj);

        /// <summary>
        ///     清理资源
        /// </summary>
        public void Dispose() => Container.Dispose();

        /// <summary>
        ///     获取当前业务注册的IOC
        /// </summary>
        public PooledList<ComponentModel> GetCustomComponent() => Container.Kernel.GraphNodes.Cast<ComponentModel>()
                                                                           .Where(predicate: model => !model.Implementation.Assembly.FullName.StartsWith(value: "Farseer.Net") &&
                                                                                                      !model.Implementation.Assembly.FullName.StartsWith(value: "Castle.")     &&
                                                                                                      !model.Implementation.Assembly.FullName.StartsWith(value: "Microsoft.")  &&
                                                                                                      !model.Implementation.Assembly.FullName.StartsWith(value: "System.")     &&
                                                                                                      model.Implementation.BaseType != typeof(FarseerModule))
                                                                           .ToPooledList();

        /// <summary>
        ///     注册自己
        /// </summary>
        private void RegisterSelf()
        {
            //Register self!
            Container.Register(Component.For<IocManager, IIocManager, IIocRegistrar, IIocResolver>().UsingFactoryMethod(factoryMethod: () => this));
        }

        /// <summary>
        ///     注册组件
        /// </summary>
        private static ComponentRegistration<T> ApplyLifestyle<T>(ComponentRegistration<T> registration, DependencyLifeStyle lifeStyle) where T : class
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Transient: return registration.LifestyleTransient();
                case DependencyLifeStyle.Singleton: return registration.LifestyleSingleton();
                default:                            return registration;
            }
        }

        /// <summary>
        ///     获取实例
        /// </summary>
        public static T GetService<T>(string name = "") => string.IsNullOrEmpty(value: name) ? Instance.Container.Resolve<T>() : Instance.Container.Resolve<T>(key: name);

        /// <summary>
        ///     获取实例
        /// </summary>
        public static T GetService<T>(Type type, string name = "") => string.IsNullOrEmpty(value: name) ? (T)Instance.Container.Resolve(service: type) : (T)Instance.Container.Resolve(key: name, service: type);

        /// <summary>
        ///     获取实例
        /// </summary>
        public static object GetService(Type type, string name = "") => string.IsNullOrEmpty(value: name) ? Instance.Container.Resolve(service: type) : Instance.Container.Resolve(key: name, service: type);

        /// <summary>
        ///     获取所有实例
        /// </summary>
        public static T[] GetServiceAll<T>() => Instance.Container.ResolveAll<T>();

        /// <summary>
        ///     获取所有实例
        /// </summary>
        public static object[] GetServiceAll(Type type) => Instance.Container.ResolveAll(service: type).Cast<object>().ToArray();

        /// <summary>
        ///     释放
        /// </summary>
        public static void ReleaseService(object obj) => Instance.Container.Release(instance: obj);
    }
}