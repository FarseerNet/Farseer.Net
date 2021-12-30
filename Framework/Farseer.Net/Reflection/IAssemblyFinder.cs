using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FS.Reflection
{
    /// <summary>
    ///     程序集查找器接口
    /// </summary>
    public interface IAssemblyFinder
    {
        /// <summary>
        ///     获取所有的程序集
        /// </summary>
        /// <returns> 程序集列表 </returns>
        List<Assembly> GetAllAssemblies();

        /// <summary>
        ///     从文件夹中获取所有的程序集
        /// </summary>
        /// <param name="folderPath"> </param>
        /// <param name="searchOption"> </param>
        List<Assembly> GetAssembliesFromFolder(string folderPath, SearchOption searchOption);
    }
}