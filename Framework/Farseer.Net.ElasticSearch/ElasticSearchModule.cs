using System.Collections.Generic;
using System.Reflection;
using Farseer.Net.Configuration;
using Farseer.Net.DI;
using Farseer.Net.ElasticSearch.Configuration;
using Farseer.Net.Modules;

namespace Farseer.Net.ElasticSearch
{
    public class ElasticSearchModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
            var configResolver = IocManager.Resolve<IConfigResolver>();
            InitConfig(configResolver);
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new ElasticSearchInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }

        private static void InitConfig(IConfigResolver configResolver)
        {
            var config = configResolver.ElasticSearchConfig();
            if (config == null || config.Items.Count == 0)
            {
                configResolver.Set(new ElasticSearchConfig { Items = new List<ElasticSearchItemConfig> { new ElasticSearchItemConfig { Name = "Default", Server = "http://10.9.15.141:9200" } } });
                configResolver.Save();
            }
        }
    }
}
