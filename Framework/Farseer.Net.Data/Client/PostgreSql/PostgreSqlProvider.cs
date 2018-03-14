using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Data.Infrastructure;
using FS.Data.Internal;

namespace FS.Data.Client.PostgreSql
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class PostgreSqlProvider : AbsDbProvider
    {
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)Assembly.Load("Npgsql").GetType("Npgsql.NpgsqlFactory").GetField("Instance").GetValue(null);//(DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("Npgsql").GetType("Npgsql.NpgsqlFactory"));
        public override AbsFunctionProvider FunctionProvider => new PostgreSqlFunctionProvider();
        public override bool IsSupportTransaction => true;
        public override string KeywordAegis(string fieldName)
        {
            //if (Regex.IsMatch(fieldName, "[\\(\\)\\,\\[\\]\\+\\= ]+")) { return fieldName; }
            return $"\"{fieldName}\"";
        }

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name) => new PostgreSqlBuilder(this, expBuilder, name);

        public override string CreateDbConnstring(string server, string port, string userID, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            var sb = new StringBuilder($"Data Source='{server}';User ID='{userID}';");
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