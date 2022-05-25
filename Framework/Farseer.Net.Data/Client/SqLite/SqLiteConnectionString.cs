using System.Text;
using FS.Configuration;

namespace FS.Data.Client.SqLite;

public class SqLiteConnectionString : AbsConnectionString
{
    public override string Create(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
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
    
    /// <summary>
    ///     获取数据库文件的路径
    /// </summary>
    /// <param name="filePath"> 数据库路径 </param>
    private string GetFilePath(string filePath)
    {
        if (filePath.IndexOf(value: ':') > -1) return filePath;

        var fileName                                  = filePath.Replace(oldValue: "/", newValue: "\\");
        if (fileName.StartsWith(value: "/")) fileName = fileName.Substring(startIndex: 1);

        fileName = SysPath.AppData + fileName;
        return fileName;
    }
    
    
    public override string GetDbName(string server) => null;
}