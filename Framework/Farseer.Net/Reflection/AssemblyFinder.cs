using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FS.Modules;
using System.Runtime.Loader;

namespace FS.Reflection
{
    /// <summary>
    /// 默认程序集查找器
    /// </summary>
    public class AssemblyFinder : IAssemblyFinder
    {
        private readonly IFarseerModuleManager _moduleManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleManager"></param>
        public AssemblyFinder(IFarseerModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }
        
        
        /// <summary>
        /// 找继承TType接口的实现类
        /// </summary>
        public Type[] GetType<TType>() => GetAllAssemblies().SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(TType)))).ToArray();

        /// <summary>
        /// 获取所有的程序集
        /// </summary>
        /// <returns></returns>
        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in _moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            assemblies.AddRange(GetAssembliesFromFolder());

            return assemblies.Distinct().ToList();
        }

        /// <summary>
        /// 在当前运行目录下，查找所有dll
        /// </summary>
        /// <returns></returns>
        public List<Assembly> GetAssembliesFromFolder()
        {
            var assemblies = new List<Assembly>();
            var files      = Directory.GetFiles($"{AppContext.BaseDirectory}", "*.dll");
            foreach (var file in files)
            {
                try
                {
                    assemblies.Add(Assembly.Load(AssemblyLoadContext.GetAssemblyName(file)));
                }
                catch (Exception)
                {
                    // ignored 失败的原因是非托管程序
                }
            }

            return assemblies;
        }


        /// <summary>
        /// 从文件夹中获取所有的程序集
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="searchOption"></param>
        public List<Assembly> GetAssembliesFromFolder(string folderPath, SearchOption searchOption)
        {
            var assemblyFiles = Directory
                .EnumerateFiles(folderPath, "*.*", searchOption)
                .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));

            return assemblyFiles.Select(o => Assembly.Load(AssemblyLoadContext.GetAssemblyName(o))).ToList();
        }
    }
}