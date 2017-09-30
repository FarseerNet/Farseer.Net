using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Farseer.Net.Configuration;
using Farseer.Net.Data.Configuration;
using Farseer.Net.Data.Infrastructure;
using Farseer.Net.Data.Internal;
using Farseer.Net.DI;

namespace Farseer.Net.Data
{
    /// <summary>
    /// Db依赖注册
    /// </summary>
    public class DataInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public DataInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var localConfigResolver = IocManager.Instance.Resolve<IConfigResolver>();

            if (localConfigResolver.Get<DbConfig>().Items.Count == 0) { return; }
            localConfigResolver.Get<DbConfig>().Items.ForEach(m =>
            {
                // 注册Db连接
                var dbConnstring = AbsDbProvider.CreateInstance(m.DataType, m.DataVer).CreateDbConnstring(m.Server, m.Port, m.UserID, m.PassWord, m.Catalog, m.DataVer, m.Additional, m.ConnectTimeout, m.PoolMinSize, m.PoolMaxSize);
                container.Register(
                    Component.For<IContextConnection>()
                    .Named(m.Name)
                    .ImplementedBy<ContextConnection>()
                    .DependsOn(
                        Dependency.OnValue(dbConnstring.GetType(), dbConnstring),
                        Dependency.OnValue(m.DataType.GetType(), m.DataType),
                        Dependency.OnValue(m.CommandTimeout.GetType(), m.CommandTimeout),
                        Dependency.OnValue(m.DataType.GetType(), m.DataType)).LifestyleSingleton());
            });
        }
    }
}
