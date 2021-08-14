using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Infrastructure;
using FS.Data.Internal;

namespace FS.Data.Client.SqLite
{
    /// <summary>
    ///     SqLite 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqLiteProvider : AbsDbProvider
    {
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("System.Data.SQLite").GetType("System.Data.SQLite.SQLiteFactory"));
        public override AbsFunctionProvider FunctionProvider => new SqLiteFunctionProvider();
        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string dbName, string tableName) => new SqLiteBuilder(this, expBuilder, dbName, tableName);
        public override bool IsSupportTransaction => true;
        public override string CreateDbConnstring(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            var sb = new StringBuilder($"Data Source='{GetFilePath(server)}';");
            if (!string.IsNullOrWhiteSpace(port)) { sb.Append($"Port='{port}';"); }

            if (!string.IsNullOrWhiteSpace(userId)) { sb.Append($"User ID='{userId}';"); }
            if (!string.IsNullOrWhiteSpace(passWord)) { sb.Append($"Password='{passWord}';"); }
            if (!string.IsNullOrWhiteSpace(dataVer)) { sb.Append($"Version='{dataVer}';"); }

            if (poolMinSize > 0) { sb.Append($"Min Pool Size='{poolMinSize}';"); }
            if (poolMaxSize > 0) { sb.Append($"Max Pool Size='{poolMaxSize}';"); }
            if (connectTimeout > 0) { sb.Append($"Connect Timeout='{connectTimeout}';"); }
            sb.Append(additional);
            return sb.ToString();
        }
    }
}