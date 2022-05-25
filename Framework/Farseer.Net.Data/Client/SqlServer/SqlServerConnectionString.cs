using System.Text;
using Castle.Core.Internal;

namespace FS.Data.Client.SqlServer;

public class SqlServerConnectionString : AbsConnectionString
{
    public override string Create(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
    {
        if (!string.IsNullOrWhiteSpace(value: port) && !server.Contains(value: ",")) server = $"{server},{port}";

        var sb = new StringBuilder(value: $"Data Source='{server}';Initial Catalog='{catalog}';");

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
    
    
    public override string GetDbName(string server)
    {
        var serverSplit   = server.Replace("'","").ToLower().Split(';');
        var database      = serverSplit.Find(o => o.StartsWith("data source"));
        var databaseSplit = database.Split('=');
        return databaseSplit.Length != 2 ? null : databaseSplit[1].Trim();
    }
}