using CacheManager.Core;

namespace FS.Cache
{
    public interface ICacheManager
    {
        /// <summary>
        /// 生成缓存对象
        /// </summary>
        /// <typeparam name="TCacheValue">缓存类型</typeparam>
        /// <param name="cacheName">缓存名称</param>
        ICacheManager<TCacheValue> Build<TCacheValue>(string cacheName);
    }
}