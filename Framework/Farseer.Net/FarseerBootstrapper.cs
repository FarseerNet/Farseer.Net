// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-20 15:22
// ********************************************

using System;
using Castle.MicroKernel.Registration;
using FS.Configuration.Startup;
using FS.DI;
using FS.DI.Installers;
using FS.Modules;

namespace FS
{
    /// <summary>
    ///     模块启动器
    /// </summary>
    public sealed class FarseerBootstrapper : IDisposable
    {
        /// <summary>
        /// 模块管理器
        /// </summary>
        private FarseerModuleManager _moduleManager;

        /// <summary>
        ///     对象是否disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        ///     启动模块
        /// </summary>
        public Type StartupModule { get; set; }

        /// <summary>
        ///     依赖注入管理器
        /// </summary>
        public IIocManager IocManager { get; set; }
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="startupModule">启动模块</param>
        /// <param name="iocManager">依赖注入管理器</param>
        private FarseerBootstrapper(Type startupModule, IIocManager iocManager)
        {
            Check.NotNull(startupModule);
            Check.NotNull(iocManager);
            Check.AssignableFrom(typeof(FarseerModule), startupModule);

            StartupModule = startupModule;
            IocManager = iocManager;
        }

        /// <summary>
        ///     清理系统
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _moduleManager?.ShutdownModules();
        }

        /// <summary>
        ///     创建系统启动器实例
        /// </summary>
        /// <typeparam name="TStartupModule">模块</typeparam>
        public static FarseerBootstrapper Create<TStartupModule>() where TStartupModule : FarseerModule => new FarseerBootstrapper(typeof(TStartupModule), DI.IocManager.Instance);

        /// <summary>
        ///     创建系统启动器
        /// </summary>
        /// <typeparam name="TStartupModule">模块</typeparam>
        /// <param name="iocManager">容器管理器</param>
        public static FarseerBootstrapper Create<TStartupModule>(IIocManager iocManager) where TStartupModule : FarseerModule => new FarseerBootstrapper(typeof(TStartupModule), iocManager);

        /// <summary>
        ///     创建启动器
        /// </summary>
        /// <param name="startupModule">模块类型</param>
        public static FarseerBootstrapper Create(Type startupModule) => new FarseerBootstrapper(startupModule, DI.IocManager.Instance);

        /// <summary>
        ///     创建启动器
        /// </summary>
        /// <param name="startupModule"></param>
        /// <param name="iocManager"></param>
        public static FarseerBootstrapper Create(Type startupModule, IIocManager iocManager) => new FarseerBootstrapper(startupModule, iocManager);

        /// <summary>
        ///     初始化系统
        /// </summary>
        public void Initialize()
        {
            try
            {
                RegisterBootstrapper();
                IocManager.Container.Install(new FarseerInstaller());
                IocManager.Resolve<FarseerStartupConfiguration>().Initialize();
                _moduleManager = IocManager.Resolve<FarseerModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                IocManager.Logger.Fatal(ex.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        ///     注册启动器
        /// </summary>
        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<FarseerBootstrapper>()) { IocManager.Container.Register(Component.For<FarseerBootstrapper>().Instance(this)); }
        }
    }
}