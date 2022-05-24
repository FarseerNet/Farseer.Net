using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Internal;
using FS.Data.Map;

namespace FS.Data.Client.ClickHouse
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class ClickHouseProvider : AbsDbProvider
    {
        //public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("Octonica.ClickHouseClient").GetType("Octonica.ClickHouseClient.ClickHouseDbProviderFactory"));
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(type: Assembly.Load(assemblyString: "ClickHouse.Client").GetType(name: "ClickHouse.Client.ADO.ClickHouseConnectionFactory"));

        public override AbsFunctionProvider FunctionProvider => new ClickHouseFunctionProvider();
        public override AbsDbParam          DbParam          => new ClickHouseParam(DbProviderFactory);
        public override AbsConnectionString ConnectionString => new ClickHouseConnectionString();

        public override bool IsSupportTransaction => false;

        public override string KeywordAegis(string fieldName) => $"`{fieldName.ToLower()}`";

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, SetDataMap setMap) => new ClickHouseBuilder(dbProvider: this, expBuilder: expBuilder, setMap);
    }
}