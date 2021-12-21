﻿using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Core.LinkTrack;
using FS.DI;

namespace FS.EC
{
    public class EnvCollectInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"> </param>
        public EnvCollectInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        ///     初始化IOC
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="store"> </param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var cts            = new CancellationTokenSource();
            var envCollectQueue = new EnvCollectQueue();
            envCollectQueue.StartDequeue(cancellationToken: cts.Token);

            container.Register(Component.For<ILinkTrackQueue>().Instance(instance: envCollectQueue).LifestyleSingleton());
        }
    }
}