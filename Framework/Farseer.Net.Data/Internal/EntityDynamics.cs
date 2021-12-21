// ********************************************
// 作者：steden QQ：11042427
// 时间：2016-08-22 10:20
// ********************************************

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FS.Core;
using FS.Core.Mapping.Attribute;
using FS.Data.Cache;
using FS.Data.Infrastructure;
using FS.Data.Map;
using FS.Extends;
using FS.Utils.Common;
using Newtonsoft.Json;

namespace FS.Data.Internal
{
    /// <summary>
    ///     实体类动态生成DataRow、IDataReader类型转换构造函数
    /// </summary>
    public class EntityDynamics
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
            var setPhysicsMap = SetMapCacheManger.Cache(key: entityType); // new SetPhysicsMap(entityType); 
            var clsName       = entityType.Name + "ByDataRow";            // 类名

            var sb = new StringBuilder();
            sb.AppendLine(value: $"public class {clsName} : {entityType.FullName}\r\n{{");
            // DataRow构造
            sb.AppendLine(value: CreateToList(entityType: entityType));
            // DataTable构造
            sb.AppendLine(value: CreateToEntity(entityType: entityType, setPhysicsMap: setPhysicsMap));

            sb.AppendLine(value: "}");

            var scriptOptions = ScriptOptions.Default
                                             .AddReferences(typeof(List<>).Assembly, typeof(ConvertHelper).Assembly, typeof(MapingData).Assembly, entityType.Assembly)
                                             .AddImports("System.Collections.Generic")
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

            var runTask  = CSharpScript.Create(code: sb.ToString(), options: scriptOptions).RunAsync();
            Task.WaitAll(runTask);
            var scriptState = runTask.Result;
            var asmName     = scriptState.Script.GetCompilation().AssemblyName;
            var assembly    = AppDomain.CurrentDomain.GetAssemblies().First(predicate: a => a.FullName.StartsWith(value: asmName, comparisonType: StringComparison.OrdinalIgnoreCase));
            return assembly.DefinedTypes.FirstOrDefault(predicate: o => o.Name == clsName);
        }

        /// <summary> 生成ToList转换方法 </summary>
        private string CreateToList(Type entityType)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(format: @"    
                            public static List<{0}> ToList(MapingData[] mapData)
                            {{
                                var lst = new List<{0}>(mapData[0].DataList.Count);
                                for (int i = 0; i < mapData[0].DataList.Count; i++) {{ lst.Add(ToEntity(mapData, i));}}
                                return lst;
                            }}", arg0: entityType.FullName);
            return sb.ToString();
        }

        /// <summary> 生成CreateToEntity转换方法 </summary>
        private string CreateToEntity(Type entityType, SetPhysicsMap setPhysicsMap)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(format: @"    
            public static {0} ToEntity(MapingData[] mapData, int rowsIndex = 0)
            {{
                if ( mapData == null || mapData.Length == 0 || mapData[0].DataList.Count == 0) {{ return null; }}
                var entity = new {0}();
                foreach (var map in mapData)
                {{
                    var col = map.DataList[rowsIndex];
                    if (col == null) {{ continue; }} 
                    switch (map.ColumnName.ToUpper())
                    {{
{1}
                    }}
                }}
                return entity;
            }}", arg0: entityType.FullName, arg1: CreateSwitchCase(setPhysicsMap: setPhysicsMap));
            return sb.ToString();
        }

        /// <summary> 生成赋值操作 </summary>
        private static string CreateSwitchCase(SetPhysicsMap setPhysicsMap)
        {
            var sb = new StringBuilder();
            foreach (var map in setPhysicsMap.MapList)
            {
                if (map.Value.Field.StorageType == EumStorageType.Ignore) continue;
                // 字段名
                var filedName = map.Value.Field.IsFun ? map.Key.Name : map.Value.Field.Name;
                // 类型转换
                var propertyType = map.Key.PropertyType.GetNullableArguments();
                // 字段赋值
                var propertyAssign = $"entity.{map.Key.Name}";

                // case 字段名
                sb.Append(value: $"\t\t\tcase \"{filedName.ToUpper()}\":\r\n\t\t\t\t");
                // 使用FS的ConvertHelper 进行类型转换泛型类型
                switch (map.Value.Field.StorageType)
                {
                    case EumStorageType.Direct:
                    {
                        // 字符串不需要处理
                        if (propertyType == typeof(string))
                            sb.Append(value: $"{propertyAssign} = col.ToString();");
                        else if (propertyType.IsEnum)
                            sb.Append(value: $"if (typeof({propertyType.FullName}).GetEnumUnderlyingType() == col.GetType()) {{ {propertyAssign} = ({propertyType.FullName})col; }} else {{ if (System.Enum.TryParse(col.ToString(), out {propertyType.FullName} {filedName}_Out)) {{ {propertyAssign} = {filedName}_Out; }} }}");
                        else if (propertyType == typeof(bool))
                            sb.Append(value: $"{propertyAssign} = ConvertHelper.ConvertType(col,false);");
                        else if (!propertyType.IsClass)
                        {
                            sb.Append(value: $"if (col is {propertyType.FullName}) {{ {propertyAssign} = ({propertyType.FullName})col; }} else {{ if ({propertyType.FullName}.TryParse(col.ToString(), out {propertyType.FullName} {filedName}_Out)) {{ {propertyAssign} = {filedName}_Out; }} }}");
                            // ClickHouse的DateTime类型，需要将UTC转为Local
                            if (setPhysicsMap.InternalContext.ContextConnection.DbType == eumDbType.ClickHouse && propertyType == typeof(DateTime)) sb.Append(value: map.Key.PropertyType.IsGenericType ? $"{propertyAssign} = {propertyAssign}.GetValueOrDefault().ToLocalTime();" : $"{propertyAssign} = {propertyAssign}.ToLocalTime();");
                        }
                        break;
                    }
                    case EumStorageType.Json:
                    {
                        if (propertyType.IsGenericType)
                        {
                            string type;
                            if (propertyType.IsArray) // 数组
                            {
                                type = $"{propertyType.GenericTypeArguments[0].FullName}[]";
                            }
                            // 字典
                            else if (propertyType.GetInterfaces().Any(o => o == typeof(IDictionary)))
                            {
                                type = $"Dictionary<{string.Join(",", propertyType.GenericTypeArguments.Select(o => o.FullName))}>";
                            }
                            else
                            {
                                type = $"List<{propertyType.GenericTypeArguments[0].FullName}>";
                            }
                            sb.Append(value: $"{propertyAssign} = JsonConvert.DeserializeObject(col.ToString(),typeof({type})) as {type}; ");
                        }

                        break;
                    }
                    case EumStorageType.Array:
                    {
                        if (propertyType.IsArray) // 数组类型
                        {
                            var asType = $"{propertyType.GetGenericArguments()[0].FullName}[]";
                            sb.Append(value: $"{propertyAssign} = ConvertHelper.ConvertType(col,typeof({asType})) as {asType}; ");
                        }
                        else // List集合
                            sb.Append(value: $"{propertyAssign} = StringHelper.ToList<{propertyType.GetGenericArguments()[0].FullName}>(col.ToString()); ");
                        break;
                    }
                }
                // 退出case
                sb.AppendLine(value: "break;");
            }

            return sb.ToString();
        }
    }
}