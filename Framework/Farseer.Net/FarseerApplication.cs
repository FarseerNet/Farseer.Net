// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-20 15:22
// ********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using FS.Configuration.Startup;
using FS.DI;
using FS.DI.Installers;
using FS.Modules;
using Microsoft.Extensions.Logging;

namespace FS
{
    /// <summary>
    ///     模块启动器
    /// </summary>
    public sealed class FarseerApplication : IDisposable
    {
        /// <summary>
        ///     对象是否disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        ///     模块管理器
        /// </summary>
        private IFarseerModuleManager _moduleManager;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="startupModule"> 启动模块 </param>
        /// <param name="iocManager"> 依赖注入管理器 </param>
        /// <param name="appName"> 应用名称 </param>
        private FarseerApplication(Type startupModule, IIocManager iocManager, string appName)
        {
            Check.NotNull(value: startupModule);
            Check.NotNull(value: iocManager);
            Check.AssignableFrom(parentType: typeof(FarseerModule), subType: startupModule);

            StartupModule = startupModule;
            IocManager    = iocManager;
            if (!string.IsNullOrWhiteSpace(value: appName)) AppName = appName;
        }

        /// <summary>
        ///     系统初始化时间
        /// </summary>
        public static DateTime StartupAt { get; private set; }

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

        private static List<Action> InitCallback { get; } = new();

        /// <summary>
        ///     清理系统
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _moduleManager?.ShutdownModules();
        }

        public static void AddInitCallback(Action act) => InitCallback.Add(item: act);

        /// <summary>
        ///     创建系统启动器实例
        /// </summary>
        /// <typeparam name="TStartupModule"> 模块 </typeparam>
        /// <param name="appName"> 应用名称 </param>
        public static FarseerApplication Run<TStartupModule>(string appName = "") where TStartupModule : FarseerModule => new(startupModule: typeof(TStartupModule), iocManager: DI.IocManager.Instance, appName: appName);

        /// <summary>
        ///     创建系统启动器
        /// </summary>
        /// <typeparam name="TStartupModule"> 模块 </typeparam>
        /// <param name="iocManager"> 容器管理器 </param>
        public static FarseerApplication Run<TStartupModule>(IIocManager iocManager, string appName = "") where TStartupModule : FarseerModule => new(startupModule: typeof(TStartupModule), iocManager: iocManager, appName: appName);

        /// <summary>
        ///     创建启动器
        /// </summary>
        /// <param name="startupModule"> 模块类型 </param>
        public static FarseerApplication Run(Type startupModule, string appName = "") => new(startupModule: startupModule, iocManager: DI.IocManager.Instance, appName: appName);

        /// <summary>
        ///     创建启动器
        /// </summary>
        /// <param name="startupModule"> </param>
        /// <param name="iocManager"> </param>
        public static FarseerApplication Run(Type startupModule, IIocManager iocManager, string appName = "") => new(startupModule: startupModule, iocManager: iocManager, appName: appName);

        /// <summary>
        ///     初始化系统
        /// </summary>
        public void Initialize()
        {
            try
            {
                StartupAt = DateTime.Now;
                RegisterBootstrapper();
                IocManager.Container.Install(new LoggerInstaller());
                IocManager.Container.Install(new FarseerInstaller());

                IocManager.Logger<FarseerApplication>().LogInformation(message: $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 注册系统核心组件");

                IocManager.Resolve<FarseerStartupConfiguration>().Initialize();
                _moduleManager = IocManager.Resolve<IFarseerModuleManager>();
                _moduleManager.Initialize(startupModule: StartupModule);
                _moduleManager.StartModules();

                // 获取业务实现类
                var lstModel = IocManager.GetCustomComponent();
                var lstLog   = new List<string>() { $"共有{lstModel.Count}个业务实例注册到容器" };
                for (var index = 0; index < lstModel.Count; index++)
                {
                    var model        = lstModel[index: index];
                    var name         = model.Name != model.Implementation.FullName ? $"{model.Name} ==>" : "";
                    var interfaceCom = model.Services.FirstOrDefault(predicate: o => o.IsInterface) ?? model.Services.FirstOrDefault();
                    lstLog.Add(interfaceCom.IsInterface ? $"{index + 1}、{name} {model.Implementation.Name} ==> {interfaceCom.Name}" : $"{index + 1}、{name} {model.Implementation.Name}");
                }
                IocManager.Logger<FarseerApplication>().LogInformation(string.Join("\r\n", lstLog));

                IocManager.Logger<FarseerApplication>().LogInformation(message: "启动初始化回调");
                foreach (var action in InitCallback) action();
                IocManager.Logger<FarseerApplication>().LogInformation(message: $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 系统初始化完毕，耗时{(DateTime.Now - StartupAt).TotalMilliseconds:n}ms");
            }
            catch (Exception ex)
            {
                IocManager.Logger<FarseerApplication>().LogError(exception: ex, message: ex.ToString());
                throw;
            }
        }

        /// <summary>
        ///     注册启动器
        /// </summary>
        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<FarseerApplication>()) IocManager.Container.Register(Component.For<FarseerApplication>().Instance(instance: this));
        }
    }
}