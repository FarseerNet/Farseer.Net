using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Data.Internal;

namespace FS.Data.Client.PostgreSql
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class PostgreSqlProvider : AbsDbProvider
    {
        public override DbProviderFactory   DbProviderFactory    => (DbProviderFactory)Assembly.Load(assemblyString: "Npgsql").GetType(name: "Npgsql.NpgsqlFactory").GetField(name: "Instance").GetValue(obj: null); //(DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("Npgsql").GetType("Npgsql.NpgsqlFactory"));
        public override AbsFunctionProvider FunctionProvider     => new PostgreSqlFunctionProvider();
        public override bool                IsSupportTransaction => true;

        public override string KeywordAegis(string fieldName) =>
            //if (Regex.IsMatch(fieldName, "[\\(\\)\\,\\[\\]\\+\\= ]+")) { return fieldName; }
            $"\"{fieldName}\"";

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string dbName, string tableName) => new PostgreSqlBuilder(dbProvider: this, expBuilder: expBuilder, dbName: dbName, tableName: tableName);

        public override string CreateDbConnstring(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            var sb = new StringBuilder(value: $"Data Source='{server}';User ID='{userId}';");
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