using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Elasticsearch.Net;
using FS.Configuration;
using FS.DI;
using FS.ElasticSearch.Configuration;
using Microsoft.Extensions.Configuration;
using Nest;

namespace FS.ElasticSearch
{
    public class ElasticSearchInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public ElasticSearchInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// 通过IOC注册ES管理接口
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var configurationSection     = IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("ElasticSearch");
            var elasticSearchItemConfigs = configurationSection.GetChildren().Select(o => o.Get<ElasticSearchItemConfig>());

            foreach (var elasticSearchItemConfig in elasticSearchItemConfigs)
            {
                var lstUrls  = elasticSearchItemConfig.Server.Split(',').Select(o => new Uri(o)).ToList();
                var settings = new ConnectionSettings(new StaticConnectionPool(lstUrls));
                
                // 注册ES实例
                container.Register(Component.For<IElasticClient>().Named($"es_{elasticSearchItemConfig.Name}").Instance(new ElasticClient(settings)).LifestyleSingleton());
            }
        }
    }
}