using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Infrastructure;
using FS.Data.Internal;

namespace FS.Data.Client.MySql
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class MySqlProvider : AbsDbProvider
    {
        //public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("MySql.Data").GetType("MySql.Data.MySqlClient.MySqlClientFactory"));
        public override DbProviderFactory   DbProviderFactory
        {
            get
            {
                return (DbProviderFactory) FieldStaticGetCacheManger.Cache(Assembly.Load("MySqlConnector").GetType("MySqlConnector.MySqlConnectorFactory").GetField("Instance"));
                //var    type = Assembly.Load("MySqlConnector").GetType("MySqlConnector.MySqlConnectorFactory");
                //var instance = type.GetField("Instance").GetValue(null);
                //return (DbProviderFactory) instance;
            }
        }

        public override AbsFunctionProvider FunctionProvider     => new MySqlFunctionProvider();
        public override bool                IsSupportTransaction => true;
        public override string KeywordAegis(string fieldName)
        {
            return $"`{fieldName}`";
        }

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder,string dbName, string tableName) => new MySqlBuilder(this, expBuilder,dbName ,tableName);

        public override string CreateDbConnstring(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            // 2016年1月13日
            // 感谢：QQ21995346 ★Master★ 同学，发现了BUG
            // 场景：连接连字符串，被强制指定了：charset='gbk'
            // 解决：移除charset='gbk'，并在DbConfig配置中增加自定义连接方式
            var sb = new StringBuilder($"Data Source='{server}';User Id='{userId}';");
            if (!string.IsNullOrWhiteSpace(port)) { sb.Append($"Port='{port}';"); }
            if (!string.IsNullOrWhiteSpace(passWord)) { sb.Append($"Password='{passWord}';"); }
            if (!string.IsNullOrWhiteSpace(catalog)) { sb.Append($"Database='{catalog}';"); }

            if (poolMinSize > 0) { sb.Append($"Min Pool Size='{poolMinSize}';"); }
            if (poolMaxSize > 0) { sb.Append($"Max Pool Size='{poolMaxSize}';"); }
            if (connectTimeout > 0) { sb.Append($"Connect Timeout='{connectTimeout}';"); }

            sb.Append(additional);
            return sb.ToString();
        }
    }
}