﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Collections.Pooled;
using FS.Modules;

namespace FS.Reflection
{
    /// <summary>
    ///     默认程序集查找器
    /// </summary>
    public class AssemblyFinder : IAssemblyFinder
    {
        private readonly IFarseerModuleManager _moduleManager;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="moduleManager"> </param>
        public AssemblyFinder(IFarseerModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        /// <summary>
        ///     获取所有的程序集
        /// </summary>
        /// <returns> </returns>
        public PooledList<Assembly> GetAllAssemblies()
        {
            using var assemblies = new PooledList<Assembly>(200);
            using var moduleManagerModules = _moduleManager.Modules;
            
            foreach (var module in moduleManagerModules)
            {
                assemblies.Add(item: module.Assembly);
                assemblies.AddRange(collection: module.Instance.GetAdditionalAssemblies());
            }

            assemblies.AddRange(collection: GetAssembliesFromFolder());

            return assemblies.Distinct().ToPooledList();
        }


        /// <summary>
        ///     从文件夹中获取所有的程序集
        /// </summary>
        /// <param name="folderPath"> </param>
        /// <param name="searchOption"> </param>
        public PooledList<Assembly> GetAssembliesFromFolder(string folderPath, SearchOption searchOption)
        {
            var assemblyFiles = Directory
                                .EnumerateFiles(path: folderPath, searchPattern: "*.*", searchOption: searchOption)
                                .Where(predicate: s => s.EndsWith(value: ".dll") || s.EndsWith(value: ".exe"));

            return assemblyFiles.Select(selector: o => Assembly.Load(assemblyRef: AssemblyLoadContext.GetAssemblyName(assemblyPath: o))).ToPooledList();
        }

        /// <summary>
        ///     在当前运行目录下，查找所有dll
        /// </summary>
        /// <returns> </returns>
        public IEnumerable<Assembly> GetAssembliesFromFolder()
        {
            var files = Directory.GetFiles(path: $"{AppContext.BaseDirectory}", searchPattern: "*.dll");
            foreach (var file in files)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load(assemblyRef: AssemblyLoadContext.GetAssemblyName(assemblyPath: file));
                }
                catch (Exception)
                {
                    // ignored 失败的原因是非托管程序
                }

                if (assembly != null) yield return assembly;
            }
        }
    }
}