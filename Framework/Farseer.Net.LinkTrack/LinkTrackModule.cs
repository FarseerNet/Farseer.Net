using System;
using System.Reflection;
using FS.DI;
using FS.ElasticSearch;
using FS.Modules;

namespace FS.LinkTrack
{
    [DependsOn(typeof(ElasticSearchModule))]
    public class LinkTrackModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
            
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new LinkTrackInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig {InstallInstallers = false});
        }
    }
}