using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Internal;

namespace FS.Data.Client.MySql
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class MySqlProvider : AbsDbProvider
    {
        //public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("MySql.Data").GetType("MySql.Data.MySqlClient.MySqlClientFactory"));
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)FieldStaticGetCacheManger.Cache(key: Assembly.Load(assemblyString: "MySqlConnector").GetType(name: "MySqlConnector.MySqlConnectorFactory").GetField(name: "Instance"));

        public override AbsFunctionProvider FunctionProvider     => new MySqlFunctionProvider();
        public override bool                IsSupportTransaction => true;

        public override string KeywordAegis(string fieldName) => $"`{fieldName}`";

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string dbName, string tableName) => new MySqlBuilder(dbProvider: this, expBuilder: expBuilder, dbName: dbName, tableName: tableName);

        public override string CreateDbConnstring(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            // 2016年1月13日
            // 感谢：QQ21995346 ★Master★ 同学，发现了BUG
            // 场景：连接连字符串，被强制指定了：charset='gbk'
            // 解决：移除charset='gbk'，并在DbConfig配置中增加自定义连接方式
            var sb = new StringBuilder(value: $"Data Source='{server}';User Id='{userId}';");
            if (!string.IsNullOrWhiteSpace(value: port)) sb.Append(value: $"Port='{port}';");
            if (!string.IsNullOrWhiteSpace(value: passWord)) sb.Append(value: $"Password='{passWord}';");
            if (!string.IsNullOrWhiteSpace(value: catalog)) sb.Append(value: $"Database='{catalog}';");

            if (poolMinSize    > 0) sb.Append(value: $"Min Pool Size='{poolMinSize}';");
            if (poolMaxSize    > 0) sb.Append(value: $"Max Pool Size='{poolMaxSize}';");
            if (connectTimeout > 0) sb.Append(value: $"Connect Timeout='{connectTimeout}';");

            sb.Append(value: additional);
            return sb.ToString();
        }
    }
}