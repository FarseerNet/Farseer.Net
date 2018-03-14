using FS.MongoDB.Configuration;
using MongoDB.Driver;

namespace FS.MongoDB
{
    /// <summary>
    /// ES管理类
    /// </summary>
    public class MongoManager : IMongoManager
    {
        private readonly MongoItemConfig _config;
        private static MongoClient _mongoClient;

        public MongoClient Client { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">配置</param>

        public MongoManager(MongoItemConfig config)
        {
            _config = config;
            Client = CreateMonggoInstance();
        }

        /// <summary>
        /// 创建ElasticClient实例
        /// </summary>
        private MongoClient CreateMonggoInstance()
        {
            if (null == _mongoClient)
            {
                //var lstUrls = _config.Server.Split(',').Select(o => new Uri(o)).ToList();
                //var pool = new StaticConnectionPool(lstUrls);
                //var settings = new ConnectionSettings(pool);
                //_elasticClient = new ElasticClient(settings);

                var conStr = _config.Server;
                _mongoClient = new MongoClient(conStr);
            }
            return _mongoClient;
        }
    }
}
