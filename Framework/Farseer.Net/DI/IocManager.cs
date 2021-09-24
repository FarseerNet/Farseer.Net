using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FS.Modules;
using Microsoft.Extensions.Logging;

namespace FS.DI
{
    /// <summary>
    ///     依赖注入管理器，用来执行依赖注入和获取的任务
    /// </summary>
    public class IocManager : IIocManager
    {
        private static readonly object objLock = new();

        /// <summary>
        ///     约定注册器列表（目前有：普用、MVC控制器）
        /// </summary>
        private readonly List<IConventionalDependencyRegistrar> _conventionalRegistrars = new();

        /// <summary>
        ///     是否已注册过WindsorInstaller
        /// </summary>
        private readonly Dictionary<Assembly, IWindsorInstaller> _isRegistrarWindsorInstaller = new();

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
        /// <param name="registrar"> </param>
        public void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar) => _conventionalRegistrars.Add(item: registrar);

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        public void RegisterAssemblyByConvention(params Assembly[] assemblys)
        {
            var ignores = new[] { "System.", "Castle.", "Farseer.Net.", "mscorlib" };
            foreach (var assembly in assemblys)
            {
                // 忽略程序集
                if (ignores.Any(predicate: ignore => assembly.FullName.StartsWith(value: ignore)) || _isRegistrarWindsorInstaller.ContainsKey(key: assembly)) continue;

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
        public void RegisterAssemblyByConvention(Type type) => RegisterAssemblyByConvention(assemblys: type.GetTypeInfo().Assembly.GetReferencedAssemblies().Select(selector: Assembly.Load).ToArray());

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        /// <param name="assembly"> </param>
        /// <param name="config"> </param>
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
        /// <typeparam name="TType"> </typeparam>
        /// <param name="name"> </param>
        /// <param name="lifeStyle"> </param>
        public void Register<TType>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class => Container.Register(string.IsNullOrEmpty(value: name) ? ApplyLifestyle(registration: Component.For<TType>(), lifeStyle: lifeStyle) : ApplyLifestyle(registration: Component.For<TType>().Named(name: name), lifeStyle: lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="name"> </param>
        /// <param name="lifeStyle"> </param>
        public void Register(Type type, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) => Container.Register(string.IsNullOrEmpty(value: name) ? ApplyLifestyle(registration: Component.For(serviceType: type), lifeStyle: lifeStyle) : ApplyLifestyle(registration: Component.For(serviceType: type).Named(name: name), lifeStyle: lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        /// <typeparam name="TType"> </typeparam>
        /// <typeparam name="TImpl"> </typeparam>
        /// <param name="name"> </param>
        /// <param name="lifeStyle"> </param>
        public void Register<TType, TImpl>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class where TImpl : class, TType => Container.Register(string.IsNullOrEmpty(value: name) ? ApplyLifestyle(registration: Component.For<TType, TImpl>().ImplementedBy<TImpl>(), lifeStyle: lifeStyle) : ApplyLifestyle(registration: Component.For<TType, TImpl>().Named(name: name).ImplementedBy<TImpl>(), lifeStyle: lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="impl"> </param>
        /// <param name="name"> </param>
        /// <param name="lifeStyle"> </param>
        public void Register(Type type, Type impl, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) => Container.Register(string.IsNullOrEmpty(value: name) ? ApplyLifestyle(registration: Component.For(type, impl).ImplementedBy(type: impl), lifeStyle: lifeStyle) : ApplyLifestyle(registration: Component.For(type, impl).Named(name: name).ImplementedBy(type: impl), lifeStyle: lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        public bool IsRegistered(Type type) => Container.Kernel.HasComponent(service: type);

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public bool IsRegistered(string name) => Container.Kernel.HasComponent(name: name);

        /// <summary>
        ///     是否注册
        /// </summary>
        /// <typeparam name="TType"> </typeparam>
        /// <returns> </returns>
        public bool IsRegistered<TType>() => Container.Kernel.HasComponent(service: typeof(TType));

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public T Resolve<T>(string name = "") => string.IsNullOrEmpty(value: name) ? Container.Resolve<T>() : Container.Resolve<T>(key: name);

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="type"> </param>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public T Resolve<T>(Type type, string name = "") => string.IsNullOrEmpty(value: name) ? (T)Container.Resolve(service: type) : (T)Container.Resolve(key: name, service: type);

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public object Resolve(Type type, string name = "") => string.IsNullOrEmpty(value: name) ? Container.Resolve(service: type) : Container.Resolve(key: name, service: type);

        /// <summary>
        ///     获取所有实例
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public T[] ResolveAll<T>() => Container.ResolveAll<T>();

        /// <summary>
        ///     获取所有实例
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        public object[] ResolveAll(Type type) => Container.ResolveAll(service: type).Cast<object>().ToArray();

        /// <summary>
        ///     释放
        /// </summary>
        /// <param name="obj"> </param>
        public void Release(object obj) => Container.Release(instance: obj);

        /// <summary>
        ///     清理资源
        /// </summary>
        public void Dispose() => Container.Dispose();

        /// <summary>
        ///     获取当前业务注册的IOC
        /// </summary>
        /// <returns> </returns>
        public List<ComponentModel> GetCustomComponent() => Container.Kernel.GraphNodes.Cast<ComponentModel>()
                                                                     .Where(predicate: model => !model.Implementation.Assembly.FullName.StartsWith(value: "Farseer.Net") &&
                                                                                                !model.Implementation.Assembly.FullName.StartsWith(value: "Castle.")     &&
                                                                                                !model.Implementation.Assembly.FullName.StartsWith(value: "Microsoft.")  &&
                                                                                                !model.Implementation.Assembly.FullName.StartsWith(value: "System.")     &&
                                                                                                model.Implementation.BaseType != typeof(FarseerModule))
                                                                     .ToList();

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
        /// <typeparam name="T"> </typeparam>
        /// <param name="registration"> </param>
        /// <param name="lifeStyle"> </param>
        /// <returns> </returns>
        private static ComponentRegistration<T> ApplyLifestyle<T>(ComponentRegistration<T> registration, DependencyLifeStyle lifeStyle) where T : class
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Transient: return registration.LifestyleTransient();
                case DependencyLifeStyle.Singleton: return registration.LifestyleSingleton();
                default:                            return registration;
            }
        }
    }
}