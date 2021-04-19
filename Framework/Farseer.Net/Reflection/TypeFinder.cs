using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Castle.Core.Logging;
using FS.DI;
using Microsoft.Extensions.Logging;

namespace FS.Reflection
{
    /// <summary>
    /// 类型查找器
    /// </summary>
    public class TypeFinder : ITypeFinder
    {
        private readonly IAssemblyFinder _assemblyFinder;
        private readonly object          _syncObj = new();
        private          Type[]          _types;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TypeFinder(IAssemblyFinder assemblyFinder)
        {
            _assemblyFinder = assemblyFinder;
        }

        /// <summary>
        /// 查找类型
        /// </summary>
        public Type[] Find(Func<Type, bool> predicate)
        {
            return GetAllTypes().Where(predicate).ToArray();
        }

        /// <summary>
        /// 查找所有的类型
        /// </summary>
        public Type[] FindAll() => GetAllTypes().ToArray();

        /// <summary>
        /// 获取所有的类型
        /// </summary>
        private Type[] GetAllTypes()
        {
            if (_types == null)
            {
                lock (_syncObj)
                {
                    if (_types == null)
                    {
                        _types = CreateTypeList().ToArray();
                    }
                }
            }

            return _types;
        }

        /// <summary>
        /// 创建类型列表
        /// </summary>
        /// <returns></returns>
        private List<Type> CreateTypeList()
        {
            var allTypes = new List<Type>();

            var assemblies = _assemblyFinder.GetAllAssemblies().Distinct();

            foreach (var assembly in assemblies)
            {
                try
                {
                    Type[] typesInThisAssembly;

                    try
                    {
                        typesInThisAssembly = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        typesInThisAssembly = ex.Types;
                    }

                    if (typesInThisAssembly.IsNullOrEmpty())
                    {
                        continue;
                    }

                    allTypes.AddRange(typesInThisAssembly.Where(type => type != null));
                }
                catch (Exception ex)
                {
                    IocManager.Instance.Logger<TypeFinder>().LogWarning(ex, ex.ToString());
                }
            }

            return allTypes;
        }
    }
}