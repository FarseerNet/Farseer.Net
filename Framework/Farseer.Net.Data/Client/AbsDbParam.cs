using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FS.Cache;
using FS.Data.Map;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Data.Client;

/// <summary>
/// 数据库参数化
/// </summary>
public abstract class AbsDbParam
{
    /// <summary>
    ///     创建提供程序对数据源类的实现的实例
    /// </summary>
    private readonly DbProviderFactory _dbProviderFactory;
    protected AbsDbParam(DbProviderFactory dbProviderFactory)
    {
        _dbProviderFactory = dbProviderFactory;
    }

    /// <summary>
    ///     是否支持参数化查询
    /// </summary>
    public virtual bool IsSupportParam => true;

    /// <summary>
    ///     参数前缀
    /// </summary>
    protected virtual string ParamsPrefix(string paramName) => $"@{paramName}";

    /// <summary>
    ///     将C#值转成数据库能存储的值
    /// </summary>
    protected virtual object ParamConvertValue(object value, DbType type)
    {
        if (value == null) return null;

        // 时间类型转换
        if (type == DbType.DateTime)
        {
            var tryResult = DateTime.TryParse(s: value.ToString(), result: out var dtValue);
            return tryResult ? value : new DateTime(year: 1900, month: 1, day: 1);
        }

        // 枚举类型转换
        if (value is Enum) return Convert.ToInt32(value: value);

        // List类型转换成字符串并以,分隔
        var valType = value.GetType();
        if (valType.IsArray || valType.IsGenericType)
        {
            var sb = new StringBuilder();
            // list类型
            if (valType.IsArray || valType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                var enumerator = ((IEnumerable)value).GetEnumerator();
                while (enumerator.MoveNext()) sb.Append(value: enumerator.Current + ",");
            }
            // 可空类型
            else if (valType.GetGenericArguments()[0] == typeof(int))
            {
                var enumerator = ((IEnumerable<int?>)value).GetEnumerator();
                while (enumerator.MoveNext()) sb.Append(value: enumerator.Current.GetValueOrDefault() + ",");
            }

            value = sb.Length > 0 ? sb.Remove(startIndex: sb.Length - 1, length: 1).ToString() : "";
        }

        return value;
    }

    /// <summary>
    ///     根据值，返回类型
    /// </summary>
    /// <param name="type"> 参数类型 </param>
    /// <param name="len"> 参数长度 </param>
    protected virtual DbType GetDbType(Type type, out int len)
    {
        type = type.GetNullableArguments();
        if (type.BaseType != null && type.BaseType == typeof(Enum))
        {
            len = 1;
            return DbType.Byte;
        }

        switch (type.Name)
        {
            case "DateTime":
                len = 8;
                return DbType.DateTime;
            case "Boolean":
                len = 1;
                return DbType.Int16; // 忘了为什么是int
            case "Int16":
                len = 2;
                return DbType.Int16;
            case "Int32":
                len = 4;
                return DbType.Int32;
            case "Int64":
                len = 8;
                return DbType.Int64;
            case "Byte":
                len = 1;
                return DbType.Byte;
            case "Long":
            case "Float":
            case "Double":
            case "Decimal":
                len = 8;
                return DbType.Decimal;
            case "Guid":
                len = 16;
                return DbType.Guid;
            default:
                len = 0;
                return DbType.String;
        }
    }

    /// <summary>
    ///     存储过程创建SQL 输入、输出参数化
    /// </summary>
    /// <typeparam name="TEntity"> 实体类 </typeparam>
    /// <param name="map"> 实体类结构 </param>
    /// <param name="entity"> 实体类 </param>
    public IEnumerable<DbParameter> InitParam<TEntity>(SetPhysicsMap map, TEntity entity) where TEntity : class, new()
    {
        if (entity == null) yield break;

        foreach (var kic in map.MapList.Where(predicate: o => o.Value.Field.IsInParam || o.Value.Field.IsOutParam))
        {
            var obj = PropertyGetCacheManger.Cache(key: kic.Key, instance: entity);
            yield return Create(name: kic.Value.Field.Name, value: obj, valType: kic.Key.PropertyType, output: kic.Value.Field.IsOutParam);
        }
    }

    /// <summary>
    ///     将OutPut参数赋值到实体
    /// </summary>
    /// <typeparam name="TEntity"> 实体类 </typeparam>
    /// <param name="map"> 实体类结构 </param>
    /// <param name="lstParam"> SQL参数列表 </param>
    /// <param name="entity"> 实体类 </param>
    public void SetParamToEntity<TEntity>(SetPhysicsMap map, IEnumerable<DbParameter> lstParam, TEntity entity) where TEntity : class, new()
    {
        if (entity == null) return;

        foreach (var kic in map.MapList.Where(predicate: o => o.Value.Field.IsOutParam))
        {
            var oVal = ConvertHelper.ConvertType(sourceValue: lstParam.FirstOrDefault(o => o.ParameterName == ParamsPrefix(paramName: kic.Value.Field.Name)).Value, returnType: kic.Key.PropertyType);
            PropertySetCacheManger.Cache(key: kic.Key, instance: entity, value: oVal);
        }
    }

    /// <summary>
    ///     创建一个数据库参数对象
    /// </summary>
    /// <param name="name"> 参数名称 </param>
    /// <param name="value"> 参数值 </param>
    /// <param name="output"> 是否是输出值 </param>
    public DbParameter Create(string name, object value, bool output = false) => Create(name: name, value: value, valType: value.GetType(), output: output);

    /// <summary>
    ///     创建一个数据库参数对象
    /// </summary>
    /// <param name="name"> 参数名称 </param>
    /// <param name="value"> 参数值 </param>
    /// <param name="valType"> 值类型 </param>
    /// <param name="output"> 是否是输出值 </param>
    /// <param name="len"> 参数长度，不指定时自动判断 </param>
    public DbParameter Create(string name, object value, Type valType, bool output = false, int len = 0)
    {
        var dbType = GetDbType(type: valType, len: out var dblen);
        return Create(name: name, value: value, type: dbType, output: output, len: len > 0 ? len : dblen);
    }

    /// <summary>
    ///     创建一个数据库参数对象
    /// </summary>
    /// <param name="name"> 参数名称 </param>
    /// <param name="value"> 参数值 </param>
    /// <param name="type"> 参数类型 </param>
    /// <param name="len"> 参数长度 </param>
    /// <param name="output"> 是否是输出值 </param>
    public virtual DbParameter Create(string name, object value, DbType type, bool output = false, int len = 0)
    {
        // DbType.String——> nvarchar
        // DbType.StringFixedLength——> nchar
        // DbType.AnsiString——> varchar
        // DbType.AnsiStringFixedLength——> char
        var param = IsSupportParam ? _dbProviderFactory.CreateParameter() : new SqlParameter();
        value = ParamConvertValue(value: value, type: type);

        param.DbType        = type;
        param.ParameterName = ParamsPrefix(paramName: Regex.Replace(input: name, pattern: "[\\(\\),=\\-\\+ ]*", replacement: ""));
        param.Value         = value;
        if (len > 0) param.Size     = len;
        if (output) param.Direction = ParameterDirection.Output;

        return param;
    }
}