using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using FS.Configuration.Startup;
using FS.DI;

namespace FS.Modules
{
    /// <summary>
    ///     模块管理器
    /// </summary>
    public class FarseerModuleManager : IFarseerModuleManager
    {
        /// <summary>
        ///     依赖注入管理器
        /// </summary>
        private readonly IIocManager _iocManager;

        /// <summary>
        ///     模块集合
        /// </summary>
        private readonly FarseerModuleCollection _moduleCollection;

        /// <summary>
        ///     启动模块类型
        /// </summary>
        private Type _startupModuleType;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocManager"></param>
        public FarseerModuleManager(IIocManager iocManager)
        {
            _moduleCollection = new FarseerModuleCollection();
            _iocManager       = iocManager;
        }

        /// <summary>
        ///     模块信息
        /// </summary>
        public FarseerModuleInfo StartupModule { get; private set; }

        /// <summary>
        ///     模块列表
        /// </summary>
        public IList<FarseerModuleInfo> Modules => _moduleCollection.ToList();

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="startupModule"></param>
        public virtual void Initialize(Type startupModule)
        {
            _startupModuleType = startupModule;
            LoadAllModules();
        }

        /// <summary>
        ///     启动模块
        /// </summary>
        public virtual void StartModules()
        {
            var sortedModules = _moduleCollection.GetListSortDependency();

            _iocManager.Logger.Info($"模块加载完毕，开始启动{sortedModules.Count}个模块...");
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());

            _iocManager.Logger.Info("模块启动完毕...");
        }

        /// <summary>
        ///     关闭模块
        /// </summary>
        public virtual void ShutdownModules()
        {
            _iocManager.Logger.Info("开始关闭模块...");

            var sortedModules = _moduleCollection.GetListSortDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());

            _iocManager.Logger.Info("模块已关闭...");
        }

        /// <summary>
        ///     加载所有模块
        /// </summary>
        private void LoadAllModules()
        {
            var moduleTypes = FindAllModules();

            _iocManager.Logger.Info($"总共找到 {moduleTypes.Count} 个模块");

            RegisterModules(moduleTypes);
            CreateModules(moduleTypes);

            FarseerModuleCollection.EnsureKernelModuleToBeFirst(_moduleCollection);

            SetDependencies();
        }

        /// <summary>
        ///     查找所有模块
        /// </summary>
        /// <returns></returns>
        private List<Type> FindAllModules()
        {
            var modules = FarseerModule.FindDependedModuleTypesRecursively(_startupModuleType);

            return modules;
        }

        /// <summary>
        ///     创建模块
        /// </summary>
        /// <param name="moduleTypes"></param>
        private void CreateModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = _iocManager.Resolve(moduleType) as FarseerModule;
                Check.NotNull<FarseerModule, FarseerInitException>(moduleObject, $"此类型不是一个有效的模块: {moduleType.AssemblyQualifiedName}");

                moduleObject.IocManager    = _iocManager;
                moduleObject.Configuration = _iocManager.Resolve<IFarseerStartupConfiguration>();

                var moduleInfo = new FarseerModuleInfo(moduleType, moduleObject);

                _moduleCollection.Add(moduleInfo);

                if (moduleType == _startupModuleType) StartupModule = moduleInfo;

                _iocManager.Logger.InfoFormat($"已经加载模块: {moduleType.AssemblyQualifiedName}");
            }
        }

        /// <summary>
        ///     注册模块
        /// </summary>
        /// <param name="moduleTypes"></param>
        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes) _iocManager.RegisterIfNot(moduleType);
        }

        /// <summary>
        ///     设置依赖
        /// </summary>
        private void SetDependencies()
        {
            foreach (var moduleInfo in _moduleCollection)
            {
                moduleInfo.Dependencies.Clear();

                foreach (var dependedModuleType in FarseerModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _moduleCollection.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null) throw new FarseerInitException($"{moduleInfo.Type.AssemblyQualifiedName}没有找到依赖的模块 {dependedModuleType.AssemblyQualifiedName}");

                    if (moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null) moduleInfo.Dependencies.Add(dependedModuleInfo);
                }
            }
        }
    }
}