// ********************************************
// 作者：steden QQ：11042427
// 时间：2016-08-22 10:20
// ********************************************

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Core;
using FS.Extends;
using Microsoft.CodeAnalysis;

namespace FS.Data.Internal
{
    /// <summary>
    ///     实体类动态生成DataRow、IDataReader类型转换构造函数
    /// </summary>
    public class DynamicCompilationEntity
    {
        private DynamicCompilationEntity() { }
        public static DynamicCompilationEntity Instance { get; } = new();

        /// <summary>
        ///     生成的派生类dll缓存起来
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type> Cache = new();
        //private static readonly object               objLock        = new();
        private static readonly PooledList<Assembly> LoadAssemblies = new() { typeof(List<>).Assembly, typeof(MapingData).Assembly, typeof(PooledList<>).Assembly };

        internal Type GetProxyType(Type entityType)
        {
            return Cache[key: entityType];
            // if (Cache.ContainsKey(key: entityType)) return Cache[key: entityType];
            //
            // lock (objLock)
            // {
            //     if (Cache.ContainsKey(key: entityType)) return Cache[key: entityType];
            //
            //     var newType = BuildEntity(entityType: entityType);
            //     Cache.TryAdd(key: entityType, value: newType);
            //     return newType;
            // }
        }

        /// <summary>
        ///     根据TEntity实体，动态生成派生类
        /// </summary>
        internal void BuildEntities(IEnumerable<Type> lstPoType)
        {
            StringBuilder sb = new();
            foreach (var entityType in lstPoType)
            {
                // 加载成员依赖的类型所在的程序集
                LoadAssembly(entityType);
                // 生成该实体类的转换代码
                sb.Append(new DynamicGenerateProxyCode(entityType).Generate());
            }
            var code = sb.ToString();

            var scriptOptions = ScriptOptions.Default
                                             .WithOptimizationLevel(OptimizationLevel.Release)
                                             .WithEmitDebugInformation(false)
                                             .AddReferences(LoadAssemblies)
                                             .AddImports("System.Collections.Generic")
                                             .AddImports("Collections.Pooled")
                                             .AddImports("System.Linq")
                                             .AddImports("Newtonsoft.Json")
                                             .AddImports("FS.Core")
                                             .AddImports("FS.Utils.Common")
                                             .AddImports("FS.Data.Abstract");

            var scriptTask = CSharpScript.Create(code, options: scriptOptions).RunAsync();
            Task.WaitAll(scriptTask);
            // 找到生成后的程序集
            var asmName  = scriptTask.Result.Script.GetCompilation().AssemblyName;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(predicate: a => a.FullName.StartsWith(value: asmName, comparisonType: StringComparison.OrdinalIgnoreCase));

            // 缓存生成后的新Type
            CacheProxyEntity(assembly, lstPoType);
        }

        /// <summary>
        /// 加载实体类引用到的程序集
        /// </summary>
        private void LoadAssembly(Type entityType)
        {
            // 先加载实体类的基类程序集
            var baseType = entityType;
            while (baseType != null)
            {
                AddAssembly(baseType.Assembly);
                baseType = baseType.BaseType;
            }

            // 加载实体类下所有属性的程序集
            var properties = entityType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                // 找到真实程序集
                var declaringType = propertyInfo.PropertyType.GetNullableArguments();

                while (declaringType.IsGenericType) declaringType = declaringType.GetGenericType();

                AddAssembly(declaringType.Assembly);
            } //AssemblyLoadContext
        }

        private void AddAssembly(Assembly assembly)
        {
            if (!LoadAssemblies.Contains(assembly)) LoadAssemblies.Add(assembly);
        }

        /// <summary>
        /// 缓存生成后的新Type
        /// </summary>
        private void CacheProxyEntity(Assembly assemblyGenerate, IEnumerable<Type> lstPoType)
        {
            foreach (var poType in lstPoType)
            {
                var dynamicGenerateProxyCode = new DynamicGenerateProxyCode(poType);
                // 找到生成后的类Type
                var proxyEntityType = assemblyGenerate.DefinedTypes.FirstOrDefault(predicate: o => o.Name == dynamicGenerateProxyCode.ClassName);
                // 缓存起来
                Cache.TryAdd(key: poType, value: proxyEntityType);
            }
        }
    }
}