using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if CORE
using System.Runtime.Loader;
#endif
using Castle.Core.Internal;
using Farseer.Net.Modules;

namespace Farseer.Net.Reflection
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
#if CORE
	        var files = Directory.GetFiles($"{AppContext.BaseDirectory}", "*.dll");
#else
			var files = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}", "*.dll");
#endif
			foreach (var file in files)
            {
	            try
	            {
#if CORE
					assemblies.Add(Assembly.Load(AssemblyLoadContext.GetAssemblyName(file)));
#else
					assemblies.Add(Assembly.LoadFrom(file));
#endif
				}
				catch (Exception)
                {
                    // ignored 失败的原因是非托管程序
                }
            }

            return assemblies;
        }
    }
}