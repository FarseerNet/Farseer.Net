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
using System.Threading.Tasks;
using FS.Core;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Data.Internal
{
    /// <summary>
    ///     实体类动态生成DataRow、IDataReader类型转换构造函数
    /// </summary>
    public class DynamicCompilationEntity
    {
        /// <summary>
        ///     生成的派生类dll缓存起来
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type> Cache = new();
        private static readonly object objLock = new();
        internal Type BuildType(Type entityType)
        {
            if (Cache.ContainsKey(key: entityType)) return Cache[key: entityType];

            lock (objLock)
            {
                if (Cache.ContainsKey(key: entityType)) return Cache[key: entityType];

                var newType = BuildEntity(entityType: entityType);
                Cache.TryAdd(key: entityType, value: newType);
                return newType;
            }
        }

        /// <summary>
        ///     根据TEntity实体，动态生成派生类
        /// </summary>
        private Type BuildEntity(Type entityType)
        {
            var generateEntityCode = new DynamicGenerateSourceCode(entityType);
            var scriptOptions = ScriptOptions.Default
                                             .AddReferences(typeof(List<>).Assembly, typeof(ConvertHelper).Assembly, typeof(MapingData).Assembly, entityType.Assembly)
                                             .AddImports("System.Collections.Generic")
                                             .AddImports("System.Linq")
                                             .AddImports("Newtonsoft.Json")
                                             .AddImports("FS.Core")
                                             .AddImports("FS.Utils.Common")
                                             .AddImports("FS.Data.Infrastructure");

            // 加载成员依赖的类型所在的程序集
            var properties = entityType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                // 找到真实程序集
                var declaringType                                 = propertyInfo.PropertyType.GetNullableArguments();
                while (declaringType.IsGenericType) declaringType = declaringType.GetGenericType();

                scriptOptions = scriptOptions.AddReferences(declaringType.Assembly);
            }

            // 需要把基类型的dll，也载进来
            var baseType = entityType;
            while (baseType != null)
            {
                scriptOptions = scriptOptions.AddReferences(baseType.Assembly);
                baseType      = baseType.BaseType;
            }

            var code = generateEntityCode.Generate();
            var runTask  = CSharpScript.Create(code, options: scriptOptions).RunAsync();
            
            Task.WaitAll(runTask);
            var scriptState = runTask.Result;
            var asmName     = scriptState.Script.GetCompilation().AssemblyName;
            var assembly    = AppDomain.CurrentDomain.GetAssemblies().First(predicate: a => a.FullName.StartsWith(value: asmName, comparisonType: StringComparison.OrdinalIgnoreCase));
            return assembly.DefinedTypes.FirstOrDefault(predicate: o => o.Name == generateEntityCode.ClassName);
            //AssemblyLoadContext
        }
    }
}