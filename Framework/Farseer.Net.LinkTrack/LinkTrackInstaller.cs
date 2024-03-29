﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Core.LinkTrack;
using FS.DI;

namespace FS.LinkTrack
{
    public class LinkTrackInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"> </param>
        public LinkTrackInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        ///     初始化IOC
        /// </summary>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILinkTrackQueue>().ImplementedBy(typeof(LinkTrackQueue)).LifestyleSingleton());
        }
    }
}