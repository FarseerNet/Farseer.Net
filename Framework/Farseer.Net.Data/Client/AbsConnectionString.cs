namespace FS.Data.Client;

/// <summary>
///     数据库连接字符串
/// </summary>
public abstract class AbsConnectionString
{
    /// <summary>
    ///     创建数据库连接字符串
    /// </summary>
    /// <param name="userId"> 账号 </param>
    /// <param name="passWord"> 密码 </param>
    /// <param name="server"> 服务器地址 </param>
    /// <param name="catalog"> 表名 </param>
    /// <param name="dataVer"> 数据库版本 </param>
    /// <param name="additional"> 自定义连接字符串 </param>
    /// <param name="connectTimeout"> 链接超时时间 </param>
    /// <param name="poolMinSize"> 连接池最小数量 </param>
    /// <param name="poolMaxSize"> 连接池最大数量 </param>
    /// <param name="port"> 端口 </param>
    public abstract string Create(string server, string port, string userId, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100);

    /// <summary>
    /// 通过连接字符串，提取dbName
    /// </summary>
    /// <param name="server">连接字符串</param>
    public abstract string GetDbName(string server);
}