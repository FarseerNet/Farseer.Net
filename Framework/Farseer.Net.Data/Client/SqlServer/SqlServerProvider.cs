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
        public override   DbProviderFactory   DbProviderFactory                                                 => SqlClientFactory.Instance;
        public override   AbsFunctionProvider FunctionProvider                                                  => new SqlServerFunctionProvider();
        public override   bool                IsSupportTransaction                                              => true;
        internal override AbsSqlBuilder       CreateSqlBuilder(ExpressionBuilder expBuilder, SetDataMap setMap) => new SqlServerBuilder(dbProvider: this, expBuilder: expBuilder, setMap);

        public override string CreateDbConnstring(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            if (!string.IsNullOrWhiteSpace(value: port) && !server.Contains(value: ",")) server = $"{server},{port}";
            var sb                                                                              = new StringBuilder(value: $"Data Source='{server}';Initial Catalog='{catalog}';");

            // 启用Windows验证方式登陆
            if (string.IsNullOrWhiteSpace(value: userId) && string.IsNullOrWhiteSpace(value: passWord))
                sb.Append(value: "Pooling=true;Integrated Security=True;");
            else
                sb.Append(value: $"User ID='{userId}';Password='{passWord}';");

            if (poolMinSize    > 0) sb.Append(value: $"Min Pool Size='{poolMinSize}';");
            if (poolMaxSize    > 0) sb.Append(value: $"Max Pool Size='{poolMaxSize}';");
            if (connectTimeout > 0) sb.Append(value: $"Connect Timeout='{connectTimeout}';");
            sb.Append(value: additional);
            return sb.ToString();
        }

        internal string[] GetCloumns(DbExecutor db, string tableName)
        {
            // 获取表结构
            var cols = db.GetDataTable(cmdType: CommandType.Text, cmdText: $"sp_columns [{tableName}]");
            return new string[0];
        }
    }

    public class TableStructure
    {
        public string FieldName   { get; set; }
        public string FieldType   { get; set; }
        public string FieldLength { get; set; }
    }
}