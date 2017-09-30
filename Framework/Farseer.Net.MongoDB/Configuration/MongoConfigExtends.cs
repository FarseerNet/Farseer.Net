using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farseer.Net.Configuration;

namespace Farseer.Net.MongoDB.Configuration
{
    public static class MongoConfigExtends
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        public static MongoConfig MongoConfig(this IConfigResolver resolver) => resolver.Get<MongoConfig>();
    }
}
