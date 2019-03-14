using System.Collections.Generic;
using System.Reflection;
using FS.Cache.Redis.Configuration;
using FS.Configuration;
using FS.DI;
using FS.Modules;

namespace FS.Cache.Redis
{
    /// <summary>
    ///     Redis模块
    /// </summary>
    [DependsOn(typeof(CacheManagerModule))]
    public class RedisModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
            // 如果Redis配置没有创建，则创建它
            var configResolver = IocManager.Resolve<IConfigResolver>();
            InitConfig(configResolver);
        }

        private void InitConfig(IConfigResolver configResolver)
        {
            var config = configResolver.RedisConfig();
            if (config == null || config.Items.Count == 0)
            {
                configResolver.Set(new RedisConfig { Items = new List<RedisItemConfig> { new RedisItemConfig { Name = "testRedis", Server = "10.9.15.32:6388,10.9.15.38:6379,10.9.15.33:6383,10.9.15.36:6389,10.9.15.34:6382,10.9.15.35:6382,syncTimeout=60000" } } });
                configResolver.Save();
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new RedisInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}