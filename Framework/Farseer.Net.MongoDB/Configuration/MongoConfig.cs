using System.Collections.Generic;
using FS.Configuration;

namespace FS.MongoDB.Configuration
{

    /// <summary>
    /// Mongo配置
    /// </summary>
    public class MongoConfig : IFarseerConfig
    {
        /// <summary>
        /// Mongo配置集合
        /// </summary>
        public List<MongoItemConfig> Items = new List<MongoItemConfig>();
    }
}
