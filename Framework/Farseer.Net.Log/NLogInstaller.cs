using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Services.Logging.NLogIntegration;
using Castle.Windsor;
using FS.Configuration;
using FS.DI;
using FS.Log.Configuration;

namespace FS.Log
{
    public class NLogInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public NLogInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// 通过IOC注册NLog管理接口
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var localConfigResolver = IocManager.Instance.Resolve<IConfigResolver>();
            if (localConfigResolver.NLogConfig()  == null) { return; }

            container.AddFacility<LoggingFacility>(f => f.LogUsing(new ExtendedNLogFactory(NLogConfigrationProvider.CreateConfigration(localConfigResolver.NLogConfig()))));
            
            //container.Register(
            //       Component.For<INLogManager>()
            //           .Named(localConfigResolver.NLogConfig().Name)
            //           .ImplementedBy<NLogManager>()
            //           //.DependsOn(Dependency.OnValue(typeof(IConfigResolver), localConfigResolver.NLogConfig()))
            //           .LifestyleSingleton());

            //localConfigResolver.NLogConfig().Items.ForEach(m =>
            //{
            //    // 注册ES连接
            //    container.Register(
            //        Component.For<INLogManager>()
            //            .Named(m.Name)
            //            .ImplementedBy<NLogManager>()
            //            .DependsOn(Dependency.OnValue(m.GetType(), m)).LifestyleSingleton());
            //});
        }
    }
}
