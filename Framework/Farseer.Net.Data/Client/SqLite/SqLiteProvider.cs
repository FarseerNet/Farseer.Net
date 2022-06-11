using System;
using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Internal;
using FS.Data.Map;

namespace FS.Data.Client.SqLite
{
    /// <summary>
    ///     SqLite 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqLiteProvider : AbsDbProvider
    {
        private static readonly Type                DbProviderFactoryType = Assembly.Load(assemblyString: "System.Data.SQLite").GetType(name: "System.Data.SQLite.SQLiteFactory");
        public override         DbProviderFactory   DbProviderFactory    => (DbProviderFactory)InstanceCacheManger.Cache(type: DbProviderFactoryType);
        public override         AbsFunctionProvider FunctionProvider     => new SqLiteFunctionProvider();
        public override         AbsDbParam          DbParam              => new SqLiteParam(DbProviderFactory);
        public override         AbsConnectionString ConnectionString     => new SqLiteConnectionString();
        public override         bool                IsSupportTransaction => true;

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, SetDataMap setMap) => new SqLiteBuilder(dbProvider: this, expBuilder: expBuilder, setMap);
    }
}