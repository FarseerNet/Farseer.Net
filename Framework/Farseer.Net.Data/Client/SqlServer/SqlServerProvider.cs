using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using FS.Data.Data;
using FS.Data.Internal;
using FS.Data.Map;

namespace FS.Data.Client.SqlServer
{
    /// <summary>
    ///     SqlServer 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqlServerProvider : AbsDbProvider
    {
        public override DbProviderFactory   DbProviderFactory    => SqlClientFactory.Instance;
        public override AbsFunctionProvider FunctionProvider     => new SqlServerFunctionProvider();
        public override AbsDbParam          DbParam              => new SqlServerParam(DbProviderFactory);
        public override AbsConnectionString ConnectionString     => new SqlServerConnectionString();
        public override bool                IsSupportTransaction => true;

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, SetDataMap setMap) => new SqlServerBuilder(dbProvider: this, expBuilder: expBuilder, setMap);

        internal string[] GetCloumns(DbExecutor db, string tableName)
        {
            // 获取表结构
            var cols = db.GetDataTable(cmdType: CommandType.Text, cmdText: $"sp_columns [{tableName}]");
            return new string[0];
        }
    }
}