using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using FS.DI;
using Microsoft.Extensions.Logging;

namespace FS.Reflection
{
    /// <summary>
    ///     类型查找器
    /// </summary>
    public class TypeFinder : ITypeFinder
    {
        private readonly IAssemblyFinder _assemblyFinder;
        private readonly object          _syncObj = new();
        private          Type[]          _types;

        /// <summary>
        ///     构造函数
        /// </summary>
        public TypeFinder(IAssemblyFinder assemblyFinder)
        {
            _assemblyFinder = assemblyFinder;
        }

        /// <summary>
        ///     查找类型
        /// </summary>
        public Type[] Find(Func<Type, bool> predicate) => GetAllTypes().Where(predicate: predicate).ToArray();

        /// <summary>
        ///     找继承TType接口的实现类
        /// </summary>
        public Type[] Find<TInterface>()
        {
            var baseType = typeof(TInterface);
            return Find(t => t.BaseType == baseType || t.GetInterfaces().Contains(baseType));
        }

        /// <summary>
        ///     查找所有的类型
        /// </summary>
        public Type[] FindAll() => GetAllTypes().ToArray();

        /// <summary>
        ///     获取所有的类型
        /// </summary>
        private Type[] GetAllTypes()
        {
            if (_types == null)
            {
                lock (_syncObj)
                {
                    if (_types == null) _types = CreateTypeList().ToArray();
                }
            }

            return _types;
        }

        private string[] _IgnoreAssembly       = { "Newtonsoft.Json.dll", "Elasticsearch.Net.dll", "Mapster.Core.dll", "Mapster.dll", "Mapster.Async.dll", "Castle.Core.dll", "Castle.Windsor.dll", "Docker.DotNet.dll", "Snowflake.Core.dll", "Nest.dll", "netstandard.dll" };
        private string[] _IgnorePrefixAssembly = { "Microsoft.Extensions.", "Microsoft.Bcl.", "Castle.Windsor.", "System.Security.", "System.Private.", "System.Configuration.", "System.Drawing.", "System.ServiceModel.", "System.Windows.", "Microsoft.IdentityModel.", "Microsoft.Win32.", "Microsoft.DotNet.", "System.ServiceProcess.", "System.IdentityModel.", "System.Diagnostics." };

        /// <summary>
        /// 忽略微软及常用的程序集
        /// </summary>
        public IEnumerable<Assembly> IgnoreAssembly(List<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (_IgnorePrefixAssembly.Any(o => assembly.ManifestModule.Name.StartsWith(o))) continue;
                if (_IgnoreAssembly.Contains(assembly.ManifestModule.Name)) continue;
                yield return assembly;
            }
        }

        /// <summary>
        ///     创建类型列表
        /// </summary>
        /// <returns> </returns>
        private List<Type> CreateTypeList()
        {
            var allTypes = new List<Type>();

            var assemblies = IgnoreAssembly(_assemblyFinder.GetAllAssemblies());

            foreach (var assembly in assemblies)
            {
                try
                {
                    if (_IgnorePrefixAssembly.Any(o => assembly.ManifestModule.Name.StartsWith(o))) continue;
                    if (_IgnoreAssembly.Contains(assembly.ManifestModule.Name)) continue;
                    Type[] typesInThisAssembly;

                    try
                    {
                        typesInThisAssembly = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        typesInThisAssembly = ex.Types;
                    }

                    if (typesInThisAssembly.IsNullOrEmpty()) continue;

                    allTypes.AddRange(collection: typesInThisAssembly.Where(predicate: type => type != null));
                }
                catch (Exception ex)
                {
                    IocManager.Instance.Logger<TypeFinder>().LogWarning(exception: ex, message: ex.ToString());
                }
            }
            return allTypes;
        }
    }
}