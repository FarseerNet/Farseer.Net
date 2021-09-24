using System.Collections.Generic;
using System.Linq;

namespace FS.Modules
{
    /// <summary>
    ///     模块集合
    /// </summary>
    internal class FarseerModuleCollection : List<FarseerModuleInfo>
    {
        /// <summary>
        ///     获取模块实例
        /// </summary>
        /// <typeparam name="TModule"> 模块类型 </typeparam>
        /// <returns> 模块实例 </returns>
        public TModule Get<TModule>() where TModule : FarseerModule
        {
            var module = this.FirstOrDefault(predicate: m => m.Type == typeof(TModule));
            Check.NotNull(value: module, parameterName: $"无法找到名为{typeof(TModule).FullName}的模块");
            return (TModule)module.Instance;
        }

        /// <summary>
        ///     根据依赖关系排序模块
        /// </summary>
        public List<FarseerModuleInfo> GetListSortDependency()
        {
            var sortedModules = SortByDependencies();
            EnsureKernelModuleToBeFirst(modules: sortedModules);
            return sortedModules;
        }

        /// <summary>
        ///     确认FarseerKernelModule模块在模块集合中第一位置
        /// </summary>
        /// <param name="modules"> </param>
        public static void EnsureKernelModuleToBeFirst(List<FarseerModuleInfo> modules)
        {
            var kernelModuleIndex = modules.FindIndex(match: m => m.Type == typeof(FarseerKernelModule));
            if (kernelModuleIndex > 0)
            {
                var kernelModule = modules[index: kernelModuleIndex];
                modules.RemoveAt(index: kernelModuleIndex);
                modules.Insert(index: 0, item: kernelModule);
            }
        }

        /// <summary>
        ///     列表排序
        /// </summary>
        private List<FarseerModuleInfo> SortByDependencies()
        {
            var sorted  = new List<FarseerModuleInfo>();
            var visited = new Dictionary<FarseerModuleInfo, bool>();

            foreach (var item in this) SortByDependenciesVisit(item: item, sorted: sorted, visited: visited);

            return sorted;
        }

        /// <summary>
        ///     根据依赖访问器排序列表
        /// </summary>
        /// <param name="item"> 元素 </param>
        /// <param name="sorted"> 排序后的列表 </param>
        /// <param name="visited"> 已经访问过的元素字典 </param>
        private static void SortByDependenciesVisit(FarseerModuleInfo item, ICollection<FarseerModuleInfo> sorted, Dictionary<FarseerModuleInfo, bool> visited)
        {
            bool inProcess;
            var  alreadyVisited = visited.TryGetValue(key: item, value: out inProcess);

            if (alreadyVisited)
                Check.IsTure(isTrue: inProcess, parameterName: "发现循环依赖！");
            else
            {
                visited[key: item] = true;

                if (item.Dependencies != null)
                {
                    foreach (var dependency in item.Dependencies) SortByDependenciesVisit(item: dependency, sorted: sorted, visited: visited);
                }

                visited[key: item] = false;
                sorted.Add(item: item);
            }
        }
    }
}