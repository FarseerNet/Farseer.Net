using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Configuration;
using FS.Core;
using FS.Data.Client;
using FS.Data.Configuration;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using Microsoft.Extensions.Configuration;

namespace FS.Data
{
    /// <summary>
    /// Db依赖注册
    /// </summary>
    public class DataInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var dbConfig = DataConfigRoot.Get();
            if (dbConfig == null || dbConfig.Items.Count == 0) return;
            dbConfig.Items.ForEach(m =>
            {
                // 注册Db连接
                var dbConnectionString = AbsDbProvider.CreateInstance(m.DataType, m.DataVer).CreateDbConnstring(m.Server, m.Port, m.UserID, m.PassWord, m.Catalog, m.DataVer, m.Additional, m.ConnectTimeout, m.PoolMinSize, m.PoolMaxSize);
                container.Register(
                    Component.For<IContextConnection>()
                        .Named($"dbConnection_{m.Name}")
                        .ImplementedBy<ContextConnection>()
                        .DependsOn(
                            Dependency.OnValue(dbConnectionString.GetType(), dbConnectionString),
                            Dependency.OnValue(m.DataType.GetType(),         m.DataType),
                            Dependency.OnValue(m.CommandTimeout.GetType(),   m.CommandTimeout),
                            Dependency.OnValue(m.DataType.GetType(),         m.DataType))
                        .LifestyleSingleton());
            });
        }
    }
}