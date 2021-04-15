using System.Collections.Generic;
using System.Reflection;
using FS.Configuration;
using FS.DI;
using FS.ElasticSearch.Configuration;
using FS.Modules;

namespace FS.ElasticSearch
{
    public class ElasticSearchModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new ElasticSearchInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}
