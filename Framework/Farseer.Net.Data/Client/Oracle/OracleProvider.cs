using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Extends;

namespace FS.Data.Client.Oracle
{
    /// <summary>
    ///     Oracle 数据库提供者（不同数据库的特性）
    /// </summary>
    public class OracleProvider : AbsDbProvider
    {
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("Oracle.ManagedDataAccess").GetType("Oracle.ManagedDataAccess.Client.OracleClientFactory"));
        public override AbsFunctionProvider FunctionProvider => new OracleFunctionProvider();
        protected override string ParamsPrefix => ":";
        public override string KeywordAegis(string fieldName) => fieldName;
        public override bool IsSupportTransaction => true;
        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder,string dbName, string tableName) => new OracleBuilder(this, expBuilder, dbName, tableName);
        public override string CreateDbConnstring(string server, string port, string userID, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            if (string.IsNullOrWhiteSpace(port)) { port = "1521"; }
            if (catalog != null && !catalog.Contains("=")) { catalog = $"SERVICE_NAME={catalog}"; }
            // SERVICE_NAME={catalog}
            // SID={catalog}
            var sb = new StringBuilder($"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={server})(PORT={port})))(CONNECT_DATA=(SERVER=DEDICATED)({catalog})));User Id={userID};Password={passWord};{additional}");

            if (poolMinSize > 0) { sb.Append($"Min Pool Size='{poolMinSize}';"); }
            if (poolMaxSize > 0) { sb.Append($"Max Pool Size='{poolMaxSize}';"); }
            if (connectTimeout > 0) { sb.Append($"Connect Timeout='{connectTimeout}';"); }
            sb.Append(additional);
            return sb.ToString();
        }
        //Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.0.131)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME = jkdb)));User Id=usercenter;Password=usercenter2013;Pooling=true;MAX Pool Size=10;Min Pool Size=1;Connection Lifetime=60;Connect Timeout=60;
        /// <summary>
        ///     根据值，返回类型
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        protected override DbType GetDbType(Type type, out int len)
        {
            type = type.GetNullableArguments();
            switch (type.Name)
            {
                //case "DateTime": len = 8; return DbType.Date;
                case "DateTime": len = 6; return DbType.DateTime;
            }
            return base.GetDbType(type, out len);
        }

        protected override object ParamConvertValue(object valu, DbType type)
        {
            // bool 值转换
            //if (valu is bool && type == DbType.Int32) { return ((bool)valu) ? 1 : 0; }
            return base.ParamConvertValue(valu, type);
        }
    }
}