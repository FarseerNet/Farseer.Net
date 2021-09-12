using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FS.Cache;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Extends;

namespace FS.Data.Client.ClickHouse
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class ClickHouseProvider : AbsDbProvider
    {
        //public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("Octonica.ClickHouseClient").GetType("Octonica.ClickHouseClient.ClickHouseDbProviderFactory"));
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("ClickHouse.Client").GetType("ClickHouse.Client.ADO.ClickHouseConnectionFactory"));

        public override AbsFunctionProvider FunctionProvider     => new ClickHouseFunctionProvider();
        public override bool                IsSupportTransaction => false;
        public override bool IsSupportParam => false;

        protected override string ParamsPrefix(string paramName) => $"{{{paramName}}}";

        public override string KeywordAegis(string fieldName)
        {
            return $"`{fieldName.ToLower()}`";
        }

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string dbName, string tableName) => new ClickHouseBuilder(this, expBuilder, dbName, tableName);

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        /// <param name="output">是否是输出值</param>
        public override DbParameter CreateDbParam(string name, object value, DbType type, bool output = false, int len = 0)
        {
            var dbParam = base.CreateDbParam(name, value, type, output, len);

            switch (type)
            {
                case DbType.String:
                    dbParam.ParameterName = $"'{dbParam.Value}'";
                    break;
                case DbType.Boolean:
                    dbParam.ParameterName = $"{dbParam.Value.ConvertType(0)}";
                    break;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    dbParam.ParameterName = $"'{(DateTime)dbParam.Value:yyyy-MM-dd HH:mm:ss}'";
                    break;
                case DbType.Int16: // 解决ch不支持bool，强制转换成int。在dbType中，bool = int16类型
                    dbParam.ParameterName = $"{dbParam.Value.ConvertType(0)}";
                    break;
                default:
                    dbParam.ParameterName = $"{dbParam.Value}";
                    break;
            }

            return dbParam;
        }

        public override string CreateDbConnstring(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            var sb = new StringBuilder($"Host={server};User={userId};Password={passWord};");
            if (!string.IsNullOrWhiteSpace(port))
            {
                sb.Append($"Port={port};");
            }

            if (!string.IsNullOrWhiteSpace(catalog))
            {
                sb.Append($"Database={catalog};");
            }

            if (!string.IsNullOrWhiteSpace(dataVer))
            {
                sb.Append($"ClientVersion={dataVer};");
            }

            if (connectTimeout > 0)
            {
                sb.Append($"CommandTimeout={connectTimeout};");
                //sb.Append($"ReadWriteTimeout={connectTimeout};");
            }

            sb.Append(additional);
            return sb.ToString();
        }
    }
}