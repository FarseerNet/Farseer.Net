using System;
using System.Text;
using FS.Configuration;
using FS.Extends;
using Microsoft.Extensions.Primitives;

namespace FS.Data.Client.SqLite;

public class SqLiteConnectionString : AbsConnectionString
{
    public override string Create(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
    {
        var sb = new StringBuilder(value: $"Data Source='{GetFilePath(filePath: server).ToString()}';");
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
    public ReadOnlySpan<char> GetFilePath(string filePath)
    {
        var spanFilePath = filePath.AsSpan();
        if (spanFilePath.IndexOf(':') > -1) return spanFilePath;

        //spanFilePath = spanFilePath.Replace('/', '\\');
        if (spanFilePath[0] == '\\') spanFilePath = spanFilePath.Slice(1);

        return SysPath.AppData.AsSpan().Concat(spanFilePath);
    }

    public override string GetDbName(string server) => null;
}