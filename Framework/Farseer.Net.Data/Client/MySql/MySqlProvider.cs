using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Internal;
using FS.Data.Map;

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
        public override AbsDbParam          DbParam              => new MySqlParam(DbProviderFactory);
        public override AbsConnectionString ConnectionString     => new MySqlConnectionString();
        public override bool                IsSupportTransaction => true;

        public override string KeywordAegis(string fieldName) => $"`{fieldName}`";

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, SetDataMap setMap) => new MySqlBuilder(dbProvider: this, expBuilder: expBuilder, setMap);
    }
}