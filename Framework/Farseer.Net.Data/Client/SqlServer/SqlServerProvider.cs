using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using FS.Data.Data;
using FS.Data.Infrastructure;
using FS.Data.Internal;

namespace FS.Data.Client.SqlServer
{
    /// <summary>
    ///     SqlServer 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqlServerProvider : AbsDbProvider
    {
        public override DbProviderFactory DbProviderFactory => SqlClientFactory.Instance;
        public override AbsFunctionProvider FunctionProvider => new SqlServerFunctionProvider();
        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder,string dbName, string tableName) => new SqlServerBuilder(this, expBuilder, dbName, tableName);
        public override bool IsSupportTransaction => true;
        public override string CreateDbConnstring(string server, string port, string userID, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            if (!string.IsNullOrWhiteSpace(port) && !server.Contains(",")) server=$"{server},{port}";
            var sb = new StringBuilder($"Data Source='{server}';Initial Catalog='{catalog}';");
            
            // 启用Windows验证方式登陆
            if (string.IsNullOrWhiteSpace(userID) && string.IsNullOrWhiteSpace(passWord)) { sb.Append("Pooling=true;Integrated Security=True;"); }
            else { sb.Append($"User ID='{userID}';Password='{passWord}';"); }

            if (poolMinSize > 0) { sb.Append($"Min Pool Size='{poolMinSize}';"); }
            if (poolMaxSize > 0) { sb.Append($"Max Pool Size='{poolMaxSize}';"); }
            if (connectTimeout > 0) { sb.Append($"Connect Timeout='{connectTimeout}';"); }
            sb.Append(additional);
            return sb.ToString();
        }

        internal string[] GetCloumns(DbExecutor db,string tableName)
        {
            // 获取表结构
            var cols = db.GetDataTable(CommandType.Text, $"sp_columns [{tableName}]");
            return new string[0];
        }
    }

    public class TableStructure
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string FieldLength { get; set; }
    }
}