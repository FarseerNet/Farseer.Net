using System;
using System.Collections;
using System.Linq;
using System.Text;
using FS.Core.Mapping.Attribute;
using FS.Data.Abstract;
using FS.Data.Cache;
using FS.Data.Map;
using FS.Extends;

namespace FS.Data.Internal;

/// <summary>
/// 动态生成代理类的源代码
/// </summary>
public class DynamicGenerateSourceCode
{
    private readonly Type          _entityType;
    private readonly SetPhysicsMap _setPhysicsMap;

    /// <summary>
    /// 类名
    /// </summary>
    public string ClassName => _entityType.Name + "ByDataRow";
    public string ParentClassName => _entityType.FullName;

    public DynamicGenerateSourceCode(Type entityType)
    {
        _entityType    = entityType;
        _setPhysicsMap = SetMapCacheManger.Cache(_entityType);
    }

    /// <summary>
    /// 生成新的Class源代码
    /// </summary>
    public string Generate()
    {
        return $@"
        public class {ClassName} : {ParentClassName}
        {{
            {GenerateToList()}
            {GenerateToEntity()}
        }}
";
    }

    /// <summary> 生成ToList转换方法 </summary>
    private string GenerateToList()
    {
        return @$"
            public static PooledList<{ParentClassName}> ToList(IEnumerable<MapingData> mapData)
            {{
                var firstData = mapData.FirstOrDefault();
                var lst = new PooledList<{ParentClassName} >(firstData.DataList.Count);
                for (int i = 0; i < firstData.DataList.Count; i++)
                {{
                    lst.Add(ToEntity(mapData, i));
                }}
                return lst;
            }}";
    }

    /// <summary> 生成CreateToEntity转换方法 </summary>
    private string GenerateToEntity()
    {
        return @$"
            public static {ParentClassName} ToEntity(IEnumerable<MapingData> mapData, int rowsIndex = 0)
            {{
                if (mapData == null || !mapData.Any() || mapData.FirstOrDefault().DataList.Count == 0) {{ return null; }}
                var entity = new {ParentClassName}();
                foreach (var map in mapData)
                {{
                    var col = map.DataList[rowsIndex];
                    if (col == null) {{ continue; }} 
                    switch (map.ColumnName.ToUpper())
                    {{
                        {GenerateSwitchCaseCode()}
                    }}
                }}
                return entity;
            }}";
    }

    /// <summary> 生成赋值操作 </summary>
    private string GenerateSwitchCaseCode()
    {
        var sb = new StringBuilder();
        // 遍历数据库字段
        foreach (var map in _setPhysicsMap.MapList)
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
                        if (_setPhysicsMap.InternalContext.DatabaseConnection.DbType == eumDbType.ClickHouse && propertyType == typeof(DateTime)) sb.Append(value: map.Key.PropertyType.IsGenericType ? $"{propertyAssign} = {propertyAssign}.GetValueOrDefault().ToLocalTime();" : $"{propertyAssign} = {propertyAssign}.ToLocalTime();");
                    }
                    break;
                }
                case EumStorageType.Json:
                {
                    string type;
                    if (propertyType.IsGenericType)
                    {
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
                    }
                    else type = propertyType.FullName;

                    sb.Append(value: $"{propertyAssign} = Jsons.ToObject<{type}>(col.ToString()); ");
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
                        sb.Append(value: $"{propertyAssign} = StringHelper.ToList<{propertyType.GetGenericArguments()[0].FullName}>(col.ToString()).ToList(); ");
                    break;
                }
            }
            // 退出case
            sb.AppendLine(value: "break;");
        }

        return sb.ToString();
    }
}