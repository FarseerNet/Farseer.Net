using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Microsoft.Extensions.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FS.Modules;

namespace FS.DI
{
    /// <summary>
    ///     依赖注入管理器，用来执行依赖注入和获取的任务
    /// </summary>
    public class IocManager : IIocManager
    {
        /// <summary>
        ///     约定注册器列表（目前有：普用、MVC控制器）
        /// </summary>
        private readonly List<IConventionalDependencyRegistrar> _conventionalRegistrars = new();

        /// <summary>
        /// 是否已注册过WindsorInstaller
        /// </summary>
        private readonly Dictionary<Assembly, IWindsorInstaller> _isRegistrarWindsorInstaller = new();

        /// <summary>
        ///     当前实例
        /// </summary>
        public static IocManager Instance { get; }

        private static object objLock = new();

        /// <summary>
        /// 日志接口,没注册时，用默认的
        /// </summary>
        public ILogger Logger<T>()
        {
            if (IsRegistered<ILogger<T>>()) return Resolve<ILogger<T>>();
            var logger = Resolve<ILoggerFactory>().CreateLogger<T>();
            lock (objLock)
            {
                if (IsRegistered<ILogger<T>>()) return Resolve<ILogger<T>>();
                Container.Register(Component.For<ILogger<T>>().Instance(logger).LifestyleSingleton());
            }

            return logger;
        }

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
        ///     构造函数
        /// </summary>
        public IocManager()
        {
            //Container = new WindsorContainer();
            //_conventionalRegistrars = new List<IConventionalDependencyRegistrar>();
        }

        /// <summary>
        ///     依赖注入容器
        /// </summary>
        public IWindsorContainer Container { get; } = new WindsorContainer();

        /// <summary>
        ///     添加约定注册器
        /// </summary>
        /// <param name="registrar"></param>
        public void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar) => _conventionalRegistrars.Add(registrar);

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        public void RegisterAssemblyByConvention(params Assembly[] assemblys)
        {
            var ignores = new[] {"System.", "Castle.", "Farseer.Net.", "mscorlib"};
            foreach (var assembly in assemblys)
            {
                // 忽略程序集
                if (ignores.Any(ignore => assembly.FullName.StartsWith(ignore)) || _isRegistrarWindsorInstaller.ContainsKey(assembly))
                {
                    continue;
                }

                try
                {
                    RegisterAssemblyByConvention(assembly, new ConventionalRegistrationConfig());
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
        public void RegisterAssemblyByConvention(Type type) => RegisterAssemblyByConvention(type.GetTypeInfo().Assembly.GetReferencedAssemblies().Select(Assembly.Load).ToArray());

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config)
        {
            var context          = new ConventionalRegistrationContext(assembly, this, config);
            var windsorInstaller = FromAssembly.Instance(assembly);
            foreach (var registerer in _conventionalRegistrars) registerer.RegisterAssembly(context);
            if (config.InstallInstallers && windsorInstaller != null)
            {
                Container.Install(windsorInstaller);
            }

            _isRegistrarWindsorInstaller.Add(assembly, windsorInstaller);
        }

        /// <summary>
        ///     注册
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="name"></param>
        /// <param name="lifeStyle"></param>
        public void Register<TType>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class => Container.Register(string.IsNullOrEmpty(name) ? ApplyLifestyle(Component.For<TType>(), lifeStyle) : ApplyLifestyle(Component.For<TType>().Named(name), lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="lifeStyle"></param>
        public void Register(Type type, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) => Container.Register(string.IsNullOrEmpty(name) ? ApplyLifestyle(Component.For(type), lifeStyle) : ApplyLifestyle(Component.For(type).Named(name), lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="name"></param>
        /// <param name="lifeStyle"></param>
        public void Register<TType, TImpl>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class where TImpl : class, TType => Container.Register(string.IsNullOrEmpty(name) ? ApplyLifestyle(Component.For<TType, TImpl>().ImplementedBy<TImpl>(), lifeStyle) : ApplyLifestyle(Component.For<TType, TImpl>().Named(name).ImplementedBy<TImpl>(), lifeStyle));

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="impl"></param>
        /// <param name="name"></param>
        /// <param name="lifeStyle"></param>
        public void Register(Type type, Type impl, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) => Container.Register(string.IsNullOrEmpty(name) ? ApplyLifestyle(Component.For(type, impl).ImplementedBy(impl), lifeStyle) : ApplyLifestyle(Component.For(type, impl).Named(name).ImplementedBy(impl), lifeStyle));

        /// <summary>
        /// 注册自己
        /// </summary>
        private void RegisterSelf()
        {
            //Register self!
            Container.Register(Component.For<IocManager, IIocManager, IIocRegistrar, IIocResolver>().UsingFactoryMethod(() => this));
        }

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsRegistered(Type type) => Container.Kernel.HasComponent(type);

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsRegistered(string name) => Container.Kernel.HasComponent(name);

        /// <summary>
        ///     是否注册
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public bool IsRegistered<TType>() => Container.Kernel.HasComponent(typeof(TType));

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Resolve<T>(string name = "") => string.IsNullOrEmpty(name) ? Container.Resolve<T>() : Container.Resolve<T>(name);

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Resolve<T>(Type type, string name = "") => string.IsNullOrEmpty(name) ? (T) Container.Resolve(type) : (T) Container.Resolve(name, type);

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Resolve(Type type, string name = "") => string.IsNullOrEmpty(name) ? Container.Resolve(type) : Container.Resolve(name, type);

        /// <summary>
        ///     获取所有实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>() => Container.ResolveAll<T>();

        /// <summary>
        ///     获取所有实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object[] ResolveAll(Type type) => Container.ResolveAll(type).Cast<object>().ToArray();

        /// <summary>
        ///     释放
        /// </summary>
        /// <param name="obj"></param>
        public void Release(object obj) => Container.Release(obj);

        /// <summary>
        ///     清理资源
        /// </summary>
        public void Dispose() => Container.Dispose();

        /// <summary>
        ///     注册组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registration"></param>
        /// <param name="lifeStyle"></param>
        /// <returns></returns>
        private static ComponentRegistration<T> ApplyLifestyle<T>(ComponentRegistration<T> registration, DependencyLifeStyle lifeStyle) where T : class
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Transient: return registration.LifestyleTransient();
                case DependencyLifeStyle.Singleton: return registration.LifestyleSingleton();
                default:
                    return registration;
            }
        }

        /// <summary>
        /// 获取当前业务注册的IOC
        /// </summary>
        /// <returns></returns>
        public List<ComponentModel> GetCustomComponent() => Container.Kernel.GraphNodes.Cast<ComponentModel>()
            .Where(model => !model.Implementation.Assembly.FullName.StartsWith("Farseer.Net") &&
                            !model.Implementation.Assembly.FullName.StartsWith("Castle.") &&
                            !model.Implementation.Assembly.FullName.StartsWith("Microsoft.") &&
                            !model.Implementation.Assembly.FullName.StartsWith("System.") &&
                            model.Implementation.BaseType != typeof(FarseerModule)).ToList();
    }
}