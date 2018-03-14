using System;
using CacheManager.Core;

namespace FS.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly Action<ConfigurationBuilderCachePart> _settings;

        public CacheManager(Action<ConfigurationBuilderCachePart> settings)
        {
            _settings = settings;
        }

        public ICacheManager<TCacheValue> Build<TCacheValue>(string cacheName) { return CacheFactory.Build<TCacheValue>(cacheName, _settings); }
    }
}