using System.Collections.Generic;
using System.Linq;
using FS.DI;
using FS.ElasticSearch.Configuration;
using Microsoft.Extensions.Configuration;

namespace Farseer.Net.Grpc.Configuration
{
    /// <summary>
    /// 读取Grpc配置
    /// </summary>
    public class GrpcConfigRoot
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        public static List<GrpcItemConfig> Get()
        {
            var configurationSection = IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("Grpc");
            return configurationSection.GetChildren().Select(o => o.Get<GrpcItemConfig>()).ToList();
        }
    }
}