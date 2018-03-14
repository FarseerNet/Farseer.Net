//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if CORE
using System.Runtime.Loader;
#endif
namespace FS.Reflection
{
	/// <summary>
	/// 程序集帮助类
	/// </summary>
	internal static class AssemblyHelper
	{
		/// <summary>
		/// 从文件夹中获取所有的程序集
		/// </summary>
		/// <param name="folderPath"></param>
		/// <param name="searchOption"></param>
		public static List<Assembly> GetAllAssembliesInFolder(string folderPath, SearchOption searchOption)
		{
			var assemblyFiles = Directory
				.EnumerateFiles(folderPath, "*.*", searchOption)
				.Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));

#if CORE
			return assemblyFiles.Select(o => Assembly.Load(AssemblyLoadContext.GetAssemblyName(o))).ToList();
#else
			return assemblyFiles.Select(Assembly.LoadFile).ToList();
#endif
		}
	}
}
