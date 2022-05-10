using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FS.DI;
using Microsoft.Extensions.Logging;

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
        /// <param name="iocManager"> </param>
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
        /// <param name="startupModule"> </param>
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
            var sw            = Stopwatch.StartNew();
            var sortedModules = _moduleCollection.GetListSortDependency();

            var lstLog = new List<string>(100) { $"Modules模块初始化..." };
            foreach (var module in sortedModules)
            {
                sw.Restart();
                module.Instance.PreInitialize();
                lstLog.Add($"耗时：{sw.ElapsedMilliseconds} ms：{module.Type.Name}.PreInitialize()");
            }
            lstLog.Add("---------------------------------------");
            foreach (var module in sortedModules)
            {
                sw.Restart();
                module.Instance.Initialize();
                lstLog.Add($"耗时：{sw.ElapsedMilliseconds} ms：{module.Type.Name}.Initialize()");
            }
            lstLog.Add("---------------------------------------");
            foreach (var module in sortedModules)
            {
                sw.Restart();
                module.Instance.PostInitialize();
                lstLog.Add($"耗时：{sw.ElapsedMilliseconds} ms：{module.Type.Name}.PostInitialize()");
            }
            lstLog.Add("---------------------------------------");
            _iocManager.Logger<FarseerModuleManager>().LogInformation(string.Join("\r\n", lstLog));
        }

        /// <summary>
        ///     关闭模块
        /// </summary>
        public virtual void ShutdownModules()
        {
            _iocManager.Logger<FarseerModuleManager>().LogInformation(message: "开始关闭模块...");

            var sortedModules = _moduleCollection.GetListSortDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(action: sm => sm.Instance.Shutdown());

            _iocManager.Logger<FarseerModuleManager>().LogInformation(message: "模块已关闭...");
        }

        /// <summary>
        ///     加载所有模块
        /// </summary>
        private void LoadAllModules()
        {
            var moduleTypes = FindAllModules();

            var lstLog = new List<string>() { $"开始加载 {moduleTypes.Count} 个模块：" };
            lstLog.AddRange(moduleTypes.Select(moduleType => $"加载模块: {moduleType.AssemblyQualifiedName}"));

            RegisterModules(moduleTypes: moduleTypes);

            CreateModules(moduleTypes: moduleTypes);

            _iocManager.Logger<FarseerModuleManager>().LogInformation(string.Join("\r\n", lstLog));

            FarseerModuleCollection.EnsureKernelModuleToBeFirst(modules: _moduleCollection);

            SetDependencies();
        }

        /// <summary>
        ///     查找所有模块
        /// </summary>
        /// <returns> </returns>
        private List<Type> FindAllModules()
        {
            var modules = FarseerModule.FindDependedModuleTypesRecursively(moduleType: _startupModuleType);

            return modules;
        }

        /// <summary>
        ///     创建模块
        /// </summary>
        /// <param name="moduleTypes"> </param>
        private void CreateModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = _iocManager.Resolve(type: moduleType) as FarseerModule;
                moduleObject.IocManager = _iocManager;
                var moduleInfo = new FarseerModuleInfo(type: moduleType, instance: moduleObject);
                _moduleCollection.Add(item: moduleInfo);
                if (moduleType == _startupModuleType) StartupModule = moduleInfo;
            }
        }

        /// <summary>
        ///     注册模块
        /// </summary>
        /// <param name="moduleTypes"> </param>
        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes) _iocManager.RegisterIfNot(type: moduleType);
        }

        /// <summary>
        ///     设置依赖
        /// </summary>
        private void SetDependencies()
        {
            foreach (var moduleInfo in _moduleCollection)
            {
                moduleInfo.Dependencies.Clear();

                foreach (var dependedModuleType in FarseerModule.FindDependedModuleTypes(moduleType: moduleInfo.Type))
                {
                    var dependedModuleInfo = _moduleCollection.FirstOrDefault(predicate: m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null) throw new FarseerInitException(message: $"{moduleInfo.Type.AssemblyQualifiedName}没有找到依赖的模块 {dependedModuleType.AssemblyQualifiedName}");

                    if (moduleInfo.Dependencies.FirstOrDefault(predicate: dm => dm.Type == dependedModuleType) == null) moduleInfo.Dependencies.Add(item: dependedModuleInfo);
                }
            }
        }
    }
}