namespace FS.Cache.Redis.Configuration;

public enum EumCommandType
{
    /// <summary>
    ///     哨兵模式
    /// </summary>
    Default,

    /// <summary>
    ///     哨兵模式
    /// </summary>
    Sentinel,

    /// <summary>
    ///     代理模式
    /// </summary>
    Twemproxy,

    /// <summary>
    ///     SSDB模式
    /// </summary>
    SSDB
}