using System.Text;
using Castle.Core.Internal;

namespace FS.Data.Client.PostgreSql;

public class PostgreSqlConnectionString : AbsConnectionString
{
    public override string Create(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
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
    
    
    public override string GetDbName(string server)
    {
        var serverSplit   = server.Replace("'","").ToLower().Split(';');
        var database      = serverSplit.Find(o => o.StartsWith("database"));
        var databaseSplit = database.Split('=');
        return databaseSplit.Length != 2 ? null : databaseSplit[1].Trim();
    }
}