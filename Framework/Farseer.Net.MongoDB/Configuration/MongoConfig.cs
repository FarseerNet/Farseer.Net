using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farseer.Net.Configuration;

namespace Farseer.Net.MongoDB.Configuration
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
