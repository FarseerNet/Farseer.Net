// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-08-02 15:28
// ********************************************
namespace FS.Cache.Configuration
{
    /// <summary>
    /// 缓存模式
    /// </summary>
    public enum EumCacheModel
    {
        /// <summary>
        /// 本地缓存
        /// </summary>
        Runtime,
        /// <summary>
        /// Redis
        /// </summary>
        Redis,
        /// <summary>
        /// 二级缓存
        /// </summary>
        RuntimeRedis,
    }
}