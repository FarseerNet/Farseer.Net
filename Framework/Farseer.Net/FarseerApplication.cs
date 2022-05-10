// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-20 15:22
// ********************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.MicroKernel.Registration;
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

        /// <summary>
        ///     应用ID（每次重启应用后会重新生成）
        /// </summary>
        public static string AppId { get; set; }

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
                AppId     = Guid.NewGuid().ToString("N");
                
                var lstLog = new List<string>(100)
                {
                    $"系统时间：{StartupAt:yyyy-MM-dd HH:mm:ss}",
                    $"进程ID：{Process.GetCurrentProcess().Id}",
                    $"应用ID：{AppId}",
                    "---------------------------------------"
                };

                var sw = Stopwatch.StartNew();
                RegisterBootstrapper();
                lstLog.Add($"耗时：{sw.ElapsedMilliseconds} ms：注册FarseerApplication组件");   

                sw.Restart();
                IocManager.Container.Install(new ConfigurationInstaller());
                lstLog.Add($"耗时：{sw.ElapsedMilliseconds} ms：注册ConfigurationInstaller组件");   
                
                sw.Restart();
                IocManager.Container.Install(new LoggerInstaller());
                lstLog.Add($"耗时：{sw.ElapsedMilliseconds} ms：注册LoggerInstaller组件");   
                
                sw.Restart();
                IocManager.Container.Install(new FarseerInstaller());
                lstLog.Add($"耗时：{sw.ElapsedMilliseconds} ms：注册FarseerInstaller组件");   
                lstLog.Add($"基础组件初始化完成");
                IocManager.Logger<FarseerModuleManager>().LogInformation(string.Join("\r\n", lstLog));
                
                _moduleManager = IocManager.Resolve<IFarseerModuleManager>();
                _moduleManager.Initialize(startupModule: StartupModule);

                _moduleManager.StartModules();

                ShowIocInstance();

                IocManager.Logger<FarseerApplication>().LogInformation(message: "启动初始化回调");
                foreach (var action in InitCallback) action();

                // 优化前：1.3s，优化后：700ms
                IocManager.Logger<FarseerApplication>().LogInformation(message: $"初始化完毕，共耗时{(DateTime.Now - StartupAt).TotalMilliseconds:n}ms");
            }
            catch (Exception ex)
            {
                IocManager.Logger<FarseerApplication>().LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 显示IOC的注册实例
        /// </summary>
        private void ShowIocInstance()
        {
            // 获取业务实现类
            var lstModel = IocManager.GetCustomComponent();
            var lstLog   = new List<string>() { $"共有{lstModel.Count}个业务实例注册到容器" };

            Dictionary<string, List<string>> dicImplName = new();

            for (var index = 0; index < lstModel.Count; index++)
            {
                var model        = lstModel[index: index];
                var interfaceCom = model.Services.FirstOrDefault(predicate: o => o.IsInterface) ?? model.Services.FirstOrDefault();
                if (interfaceCom.IsInterface)
                {
                    if (!dicImplName.ContainsKey(interfaceCom.Name)) dicImplName[interfaceCom.Name] = new();
                    dicImplName[interfaceCom.Name].Add(model.Implementation.Name);
                }
            }

            lstLog.AddRange(dicImplName.Select(impl => $"{impl.Key}\t---->\t{string.Join("|", impl.Value)}"));

            IocManager.Logger<FarseerApplication>().LogInformation(string.Join("\r\n", lstLog));
        }

        /// <summary>
        ///     注册启动器
        /// </summary>
        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<FarseerApplication>())
            {
                IocManager.Container.Register(Component.For<FarseerApplication>().Instance(instance: this));
            }
        }
    }
}