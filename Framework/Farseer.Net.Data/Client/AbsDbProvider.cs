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
using FS.Configuration;
using FS.Data.Client.ClickHouse;
using FS.Data.Client.MySql;
using FS.Data.Client.PostgreSql;
using FS.Data.Client.SqLite;
using FS.Data.Client.SqlServer;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Data.Map;
using FS.Extends;
using FS.Utils.Common;
#if !CORE
using FS.Data.Client.OleDb;
using FS.Data.Client.Oracle;
#endif

namespace FS.Data.Client
{
    /// <summary>
    ///     数据库提供者（不同数据库的特性）
    /// </summary>
    public abstract class AbsDbProvider
    {
        /// <summary>
        ///     是否支持参数化查询
        /// </summary>
        public virtual bool IsSupportParam => true;

        /// <summary>
        ///     是否支持事务操作
        /// </summary>
        public abstract bool IsSupportTransaction { get; }

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public abstract DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public abstract AbsFunctionProvider FunctionProvider { get; }

        /// <summary>
        ///     参数前缀
        /// </summary>
        protected virtual string ParamsPrefix(string paramName) => $"@{paramName}";


        /// <summary>
        ///     创建字段保护符
        /// </summary>
        /// <param name="fieldName"> 字符名称 </param>
        public virtual string KeywordAegis(string fieldName) =>
        //if (Regex.IsMatch(fieldName, "[\\(\\)\\,\\[\\]\\+\\= ]+")) { return fieldName; }
        $"[{fieldName}]";

        #region 返回DbProvider

        /// <summary>
        ///     返回数据库类型名称
        /// </summary>
        /// <param name="dbType"> 数据库类型 </param>
        /// <param name="dataVer"> 数据库版本 </param>
        public static AbsDbProvider CreateInstance(eumDbType dbType, string dataVer = null)
        {
            switch (dbType)
            {
#if !CORE
                case eumDbType.OleDb: return new OleDbProvider();
                case eumDbType.Oracle: return new OracleProvider();
#endif
                case eumDbType.MySql:      return new MySqlProvider();
                case eumDbType.ClickHouse: return new ClickHouseProvider();
                case eumDbType.SQLite:     return new SqLiteProvider();
                case eumDbType.PostgreSql: return new PostgreSqlProvider();
            }

            return new SqlServerProvider();
        }

        #endregion

        /// <summary>
        ///     创建SQL查询
        /// </summary>
        /// <param name="expBuilder"> 表达式持久化 </param>
        /// <param name="setMap">实体类结构映射 </param>
        internal abstract AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, SetDataMap setMap);

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
                yield return CreateDbParam(name: kic.Value.Field.Name, value: obj, valType: kic.Key.PropertyType, output: kic.Value.Field.IsOutParam);
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

        #region 创建参数

        /// <summary>
        ///     将C#值转成数据库能存储的值
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="type"> </param>
        /// <returns> </returns>
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
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name"> 参数名称 </param>
        /// <param name="value"> 参数值 </param>
        /// <param name="output"> 是否是输出值 </param>
        public DbParameter CreateDbParam(string name, object value, bool output = false) => CreateDbParam(name: name, value: value, valType: value.GetType(), output: output);

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name"> 参数名称 </param>
        /// <param name="value"> 参数值 </param>
        /// <param name="valType"> 值类型 </param>
        /// <param name="output"> 是否是输出值 </param>
        /// <param name="len"> 参数长度，不指定时自动判断 </param>
        public DbParameter CreateDbParam(string name, object value, Type valType, bool output = false, int len = 0)
        {
            var dbType = GetDbType(type: valType, len: out var dblen);
            return CreateDbParam(name: name, value: value, type: dbType, output: output, len: len > 0 ? len : dblen);
        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name"> 参数名称 </param>
        /// <param name="value"> 参数值 </param>
        /// <param name="type"> 参数类型 </param>
        /// <param name="len"> 参数长度 </param>
        /// <param name="output"> 是否是输出值 </param>
        public virtual DbParameter CreateDbParam(string name, object value, DbType type, bool output = false, int len = 0)
        {
            // DbType.String——> nvarchar
            // DbType.StringFixedLength——> nchar
            // DbType.AnsiString——> varchar
            // DbType.AnsiStringFixedLength——> char
            var param = IsSupportParam ? DbProviderFactory.CreateParameter() : new SqlParameter();
            value = ParamConvertValue(value: value, type: type);

            param.DbType        = type;
            param.ParameterName = ParamsPrefix(paramName: Regex.Replace(input: name, pattern: "[\\(\\),=\\-\\+ ]*", replacement: ""));
            param.Value         = value;
            if (len > 0) param.Size     = len;
            if (output) param.Direction = ParameterDirection.Output;

            return param;
        }

        #endregion

        #region 返回数据库连接字符串

        /// <summary>
        ///     创建数据库连接字符串
        /// </summary>
        /// <param name="userId"> 账号 </param>
        /// <param name="passWord"> 密码 </param>
        /// <param name="server"> 服务器地址 </param>
        /// <param name="catalog"> 表名 </param>
        /// <param name="dataVer"> 数据库版本 </param>
        /// <param name="additional"> 自定义连接字符串 </param>
        /// <param name="connectTimeout"> 链接超时时间 </param>
        /// <param name="poolMinSize"> 连接池最小数量 </param>
        /// <param name="poolMaxSize"> 连接池最大数量 </param>
        /// <param name="port"> 端口 </param>
        public abstract string CreateDbConnstring(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100);

        /// <summary>
        ///     获取数据库文件的路径
        /// </summary>
        /// <param name="filePath"> 数据库路径 </param>
        protected string GetFilePath(string filePath)
        {
            if (filePath.IndexOf(value: ':') > -1) return filePath;

            var fileName                                  = filePath.Replace(oldValue: "/", newValue: "\\");
            if (fileName.StartsWith(value: "/")) fileName = fileName.Substring(startIndex: 1);

            fileName = SysPath.AppData + fileName;
            return fileName;
        }

        #endregion
    }
}