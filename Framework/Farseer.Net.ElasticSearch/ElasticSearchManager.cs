using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using Nest;
using Farseer.Net.ElasticSearch.Configuration;
using System.Linq;

namespace Farseer.Net.ElasticSearch
{
    /// <summary>
    /// ES管理类
    /// </summary>
    public class ElasticSearchManager : IElasticSearchManager
    {
        private readonly ElasticSearchItemConfig _config;
        private static ElasticClient _elasticClient;

        public ElasticClient Client { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">ES配置</param>

        public ElasticSearchManager(ElasticSearchItemConfig config)
        {
            _config = config;
            Client = CreateEcInstance();
        }

        /// <summary>
        /// 创建ElasticClient实例
        /// </summary>
        private ElasticClient CreateEcInstance()
        {
            if (null == _elasticClient)
            {
                var lstUrls = _config.Server.Split(',').Select(o => new Uri(o)).ToList();
                var pool = new StaticConnectionPool(lstUrls);
                var settings = new ConnectionSettings(pool);
                _elasticClient = new ElasticClient(settings);
            }
            return _elasticClient;
        }
    }
}
