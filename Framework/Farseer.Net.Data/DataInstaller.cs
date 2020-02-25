using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Configuration;
using FS.Core;
using FS.Data.Client;
using FS.Data.Configuration;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.DI;

namespace FS.Data
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
            var config = localConfigResolver.Get<DbConfig>();
            if (config.Items.Count == 0) { return; }
            config.Items.ForEach(m =>
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

            // 代理异常记录
            if (config.IsWriteSqlErrorLog) container.Register(Component.For<ISqlMonitor>().ImplementedBy<MonitorSqlExceptionLog>().LifestyleTransient());
            // 代理SQL记录
            if (config.IsWriteSqlRunLog) container.Register(Component.For<ISqlMonitor>().ImplementedBy<MonitorSqlLog>().LifestyleTransient());
        }
    }
}
