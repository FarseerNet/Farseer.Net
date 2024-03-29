﻿using System.Reflection;
using FS.DI;
using FS.ElasticSearch;
using FS.Modules;
using FS.MQ.Queue;

namespace FS.LinkTrack
{
    [DependsOn(typeof(ElasticSearchModule),typeof(QueueModule))]
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
            IocManager.Container.Install(new LinkTrackInstaller(iocResolver: IocManager));
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}