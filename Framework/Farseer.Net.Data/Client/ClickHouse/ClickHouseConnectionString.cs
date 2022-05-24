using System.Text;

namespace FS.Data.Client.ClickHouse;

public class ClickHouseConnectionString : AbsConnectionString
{
    public override string Create(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
    {
        var sb = new StringBuilder(value: $"Host={server};User={userId};Password={passWord};");
        if (!string.IsNullOrWhiteSpace(value: port)) sb.Append(value: $"Port={port};");

        if (!string.IsNullOrWhiteSpace(value: catalog)) sb.Append(value: $"Database={catalog};");

        if (!string.IsNullOrWhiteSpace(value: dataVer)) sb.Append(value: $"ClientVersion={dataVer};");

        if (connectTimeout > 0)
        {
            sb.Append(value: $"CommandTimeout={connectTimeout};");
            //sb.Append($"ReadWriteTimeout={connectTimeout};");
        }

        sb.Append(value: additional);
        return sb.ToString();
    }
}