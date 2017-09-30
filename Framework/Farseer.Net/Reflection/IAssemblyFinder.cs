using System.Collections.Generic;
using System.Reflection;

namespace Farseer.Net.Reflection
{
    /// <summary>
    /// 程序集查找器接口
    /// </summary>
    public interface IAssemblyFinder
    {
        /// <summary>
        /// 获取所有的程序集
        /// </summary>
        /// <returns>程序集列表</returns>
        List<Assembly> GetAllAssemblies();
    }
}