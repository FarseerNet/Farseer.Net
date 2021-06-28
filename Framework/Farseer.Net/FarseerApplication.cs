// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-20 15:22
// ********************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core;
using Castle.Core.Internal;
using Castle.Windsor;
using FS.Configuration.Startup;
using FS.DI;
using FS.DI.Installers;
using FS.Log;
using FS.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Component = Castle.MicroKernel.Registration.Component;

namespace FS
{
    /// <summary>
    ///     模块启动器
    /// </summary>
    public sealed class FarseerApplication : IDisposable
    {
        /// <summary>
        /// 模块管理器
        /// </summary>
        private IFarseerModuleManager _moduleManager;

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
        ///     应用名称
        /// </summary>
        public static string AppName { get; set; }

        private static List<Action> InitCallback { get; set; } = new();

        public static void AddInitCallback(Action act) => InitCallback.Add(act);

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="startupModule">启动模块</param>
        /// <param name="iocManager">依赖注入管理器</param>
        /// <param name="appName">应用名称</param>
        private FarseerApplication(Type startupModule, IIocManager iocManager, string appName)
        {
            Check.NotNull(startupModule);
            Check.NotNull(iocManager);
            Check.AssignableFrom(typeof(FarseerModule), startupModule);

            StartupModule = startupModule;
            IocManager    = iocManager;
            if (!string.IsNullOrWhiteSpace(appName)) AppName = appName;
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
        /// <param name="appName">应用名称</param>
        public static FarseerApplication Run<TStartupModule>(string appName = "") where TStartupModule : FarseerModule
        {
            return new(typeof(TStartupModule), DI.IocManager.Instance, appName);
        }

        /// <summary>
        ///     创建系统启动器
        /// </summary>
        /// <typeparam name="TStartupModule">模块</typeparam>
        /// <param name="iocManager">容器管理器</param>
        public static FarseerApplication Run<TStartupModule>(IIocManager iocManager, string appName = "") where TStartupModule : FarseerModule => new(typeof(TStartupModule), iocManager, appName);

        /// <summary>
        ///     创建启动器
        /// </summary>
        /// <param name="startupModule">模块类型</param>
        public static FarseerApplication Run(Type startupModule, string appName = "") => new(startupModule, DI.IocManager.Instance, appName);

        /// <summary>
        ///     创建启动器
        /// </summary>
        /// <param name="startupModule"></param>
        /// <param name="iocManager"></param>
        public static FarseerApplication Run(Type startupModule, IIocManager iocManager, string appName = "") => new(startupModule, iocManager, appName);

        /// <summary>
        ///     初始化系统
        /// </summary>
        public void Initialize(Action<ILoggingBuilder> configure = null)
        {
            try
            {
                var dt = DateTime.Now;
                RegisterBootstrapper();
                IocManager.Container.Install(new LoggerInstaller(configure));
                IocManager.Container.Install(new FarseerInstaller());

                IocManager.Logger<FarseerApplication>().LogInformation("注册系统核心组件");

                IocManager.Resolve<FarseerStartupConfiguration>().Initialize();
                _moduleManager = IocManager.Resolve<IFarseerModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();

                IocManager.Logger<FarseerApplication>().LogInformation("启动初始化回调");
                foreach (var action in InitCallback)
                {
                    action();
                }

                IocManager.Logger<FarseerApplication>().LogInformation($"系统初始化完毕，耗时{(DateTime.Now - dt).TotalMilliseconds:n}ms");

                // 获取业务实现类
                var lstModel = IocManager.GetCustomComponent();
                Console.WriteLine($"共有{lstModel.Count}个业务实例注册到容器");
                for (int index = 0; index < lstModel.Count; index++)
                {
                    var model        = lstModel[index];
                    var name         = model.Name != model.Implementation.FullName ? $"{model.Name} ==>" : "";
                    var interfaceCom = model.Services.FirstOrDefault(o => o.IsInterface) ?? model.Services.FirstOrDefault();
                    Console.WriteLine(interfaceCom.IsInterface ? $"{index + 1}、{name} {model.Implementation.Name} ==> {interfaceCom.Name}" : $"{index + 1}、{name} {model.Implementation.Name}");
                }
            }
            catch (Exception ex)
            {
                IocManager.Logger<FarseerApplication>().LogError(ex, ex.ToString());
                throw;
            }
        }

        /// <summary>
        ///     注册启动器
        /// </summary>
        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<FarseerApplication>())
            {
                IocManager.Container.Register(Component.For<FarseerApplication>().Instance(this));
            }
        }
    }
}