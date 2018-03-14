using System.Collections.Generic;
using System.Reflection;
using FS.Configuration;
using FS.DI;
using FS.Modules;
using FS.MongoDB.Configuration;

namespace FS.MongoDB
{
    public class MongoModule : FarseerModule
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
            IocManager.Container.Install(new MongoInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }

        private static void InitConfig(IConfigResolver configResolver)
        {
            var config = configResolver.MongoConfig();
            if (config == null || config.Items.Count == 0)
            {
                configResolver.Set(new MongoConfig { Items = new List<MongoItemConfig> { new MongoItemConfig { Name = "Default", Server = "mongodb://127.0.0.1:27017" } } });
                configResolver.Save();
            }
        }
    }
}
