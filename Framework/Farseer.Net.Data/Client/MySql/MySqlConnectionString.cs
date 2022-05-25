using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Internal;

namespace FS.Data.Client.MySql;

public class MySqlConnectionString : AbsConnectionString
{
    public override string Create(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
    {
        // 2016年1月13日
        // 感谢：QQ21995346 ★Master★ 同学，发现了BUG
        // 场景：连接连字符串，被强制指定了：charset='gbk'
        // 解决：移除charset='gbk'，并在DbConfig配置中增加自定义连接方式
        var sb = new StringBuilder(value: $"Data Source='{server}';User Id='{userId}';");
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
        var serverSplit = server.Replace("'","").ToLower().Split(';');
        var database    = serverSplit.Find(o => o.StartsWith("database"));
        var databaseSplit     = database.Split('=');
        return databaseSplit.Length != 2 ? null : databaseSplit[1].Trim();
    }
}