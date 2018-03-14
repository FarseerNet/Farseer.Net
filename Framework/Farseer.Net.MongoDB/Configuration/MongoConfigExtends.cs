using FS.Configuration;

namespace FS.MongoDB.Configuration
{
    public static class MongoConfigExtends
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        public static MongoConfig MongoConfig(this IConfigResolver resolver) => resolver.Get<MongoConfig>();
    }
}
