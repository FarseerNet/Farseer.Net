// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 16:16
// ********************************************

namespace FS.Cache.Redis.Configuration;

/// <summary>
///     Redis配置
/// </summary>
public class RedisItemConfig
{
    /// <summary> 集群名称 </summary>
    public string Name { get; set; }

    /// <summary> 集群IP:Port地址 </summary>
    public string Server { get; set; }

    /// <summary> DB </summary>
    public int DB { get; set; }

    /// <summary> 密码 </summary>
    public string Password { get; set; }

    /// <summary> 连接超时 </summary>
    public int ConnectTimeout { get; set; }

    /// <summary> 超时 </summary>
    public int SyncTimeout { get; set; }

    /// <summary> 超时 </summary>
    public int ResponseTimeout { get; set; }

    /// <summary> 命令类型（默认集群模式） </summary>
    public EumCommandType CommandType { get; set; }

    /// <summary> 哨兵模式下需要填写 </summary>
    public string ServiceName { get; set; }

    public string TieBreaker { get; set; }
}