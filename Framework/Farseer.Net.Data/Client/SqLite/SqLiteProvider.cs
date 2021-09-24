using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Internal;

namespace FS.Data.Client.SqLite
{
    /// <summary>
    ///     SqLite 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqLiteProvider : AbsDbProvider
    {
        public override   DbProviderFactory   DbProviderFactory                                                               => (DbProviderFactory)InstanceCacheManger.Cache(type: Assembly.Load(assemblyString: "System.Data.SQLite").GetType(name: "System.Data.SQLite.SQLiteFactory"));
        public override   AbsFunctionProvider FunctionProvider                                                                => new SqLiteFunctionProvider();
        public override   bool                IsSupportTransaction                                                            => true;
        internal override AbsSqlBuilder       CreateSqlBuilder(ExpressionBuilder expBuilder, string dbName, string tableName) => new SqLiteBuilder(dbProvider: this, expBuilder: expBuilder, dbName: dbName, tableName: tableName);

        public override string CreateDbConnstring(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            var sb = new StringBuilder(value: $"Data Source='{GetFilePath(filePath: server)}';");
            if (!string.IsNullOrWhiteSpace(value: port)) sb.Append(value: $"Port='{port}';");

            if (!string.IsNullOrWhiteSpace(value: userId)) sb.Append(value: $"User ID='{userId}';");
            if (!string.IsNullOrWhiteSpace(value: passWord)) sb.Append(value: $"Password='{passWord}';");
            if (!string.IsNullOrWhiteSpace(value: dataVer)) sb.Append(value: $"Version='{dataVer}';");

            if (poolMinSize    > 0) sb.Append(value: $"Min Pool Size='{poolMinSize}';");
            if (poolMaxSize    > 0) sb.Append(value: $"Max Pool Size='{poolMaxSize}';");
            if (connectTimeout > 0) sb.Append(value: $"Connect Timeout='{connectTimeout}';");
            sb.Append(value: additional);
            return sb.ToString();
        }
    }
}