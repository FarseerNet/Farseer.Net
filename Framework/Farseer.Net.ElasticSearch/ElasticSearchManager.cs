using System;
using System.Linq;
using Elasticsearch.Net;
using FS.ElasticSearch.Configuration;
using Nest;

namespace FS.ElasticSearch
{
    /// <summary>
    /// ES管理类
    /// </summary>
    public class ElasticSearchManager : IElasticSearchManager
    {
        public ElasticClient Client { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">ES配置</param>
        public ElasticSearchManager(ElasticSearchItemConfig config)
        {
            var lstUrls = config.Server.Split(',').Select(o => new Uri(o)).ToList();
            var pool = new StaticConnectionPool(lstUrls);
            var settings = new ConnectionSettings(pool);
            Client = new ElasticClient(settings);
        }
    }
}