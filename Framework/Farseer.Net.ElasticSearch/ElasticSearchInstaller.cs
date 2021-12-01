using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Elasticsearch.Net;
using FS.DI;
using FS.ElasticSearch.Configuration;
using Nest;

namespace FS.ElasticSearch
{
    public class ElasticSearchInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"> </param>
        public ElasticSearchInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        ///     通过IOC注册ES管理接口
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="store"> </param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var elasticSearchItemConfigs = ElasticSearchConfigRoot.Get();
            if (elasticSearchItemConfigs == null || !elasticSearchItemConfigs.Any()) return;
            foreach (var elasticSearchItemConfig in elasticSearchItemConfigs)
            {
                if (string.IsNullOrWhiteSpace(elasticSearchItemConfig.Server)) throw new FarseerException($"Elasticsearch配置:{elasticSearchItemConfig.Name}，缺少Server节点");
                
                var lstUrls  = elasticSearchItemConfig.Server.Split(',').Select(selector: o => new Uri(uriString: o)).ToList();
                var settings = new ConnectionSettings(connectionPool: new StaticConnectionPool(uris: lstUrls));
                // 如果设置了用户名，则附加鉴权设置
                if (!string.IsNullOrWhiteSpace(value: elasticSearchItemConfig.Username) || !string.IsNullOrWhiteSpace(value: elasticSearchItemConfig.Password)) settings.BasicAuthentication(username: elasticSearchItemConfig.Username, password: elasticSearchItemConfig.Password);
                // 注册ES实例
                container.Register(Component.For<IElasticClient>().Named(name: elasticSearchItemConfig.Name).Instance(instance: new ElasticClient(connectionSettings: settings)).LifestyleSingleton());
            }
        }
    }
}