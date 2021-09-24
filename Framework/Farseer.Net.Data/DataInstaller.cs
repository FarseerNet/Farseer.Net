using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Data.Client;
using FS.Data.Configuration;
using FS.Data.Internal;

namespace FS.Data
{
    /// <summary>
    ///     Db依赖注册
    /// </summary>
    public class DataInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     注册依赖
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="store"> </param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var dbConfig = DataConfigRoot.Get();
            if (dbConfig == null || dbConfig.Items.Count == 0) return;
            dbConfig.Items.ForEach(action: m =>
            {
                // 注册Db连接
                var dbConnectionString = AbsDbProvider.CreateInstance(dbType: m.DataType, dataVer: m.DataVer).CreateDbConnstring(server: m.Server, port: m.Port, userId: m.UserID, passWord: m.PassWord, catalog: m.Catalog, dataVer: m.DataVer, additional: m.Additional, connectTimeout: m.ConnectTimeout, poolMinSize: m.PoolMinSize, poolMaxSize: m.PoolMaxSize);
                container.Register(
                                   Component.For<IContextConnection>()
                                            .Named(name: $"dbConnection_{m.Name}")
                                            .ImplementedBy<ContextConnection>()
                                            .DependsOn(
                                                       Dependency.OnValue(dependencyType: dbConnectionString.GetType(), value: dbConnectionString),
                                                       Dependency.OnValue(dependencyType: m.DataType.GetType(), value: m.DataType),
                                                       Dependency.OnValue(dependencyType: m.CommandTimeout.GetType(), value: m.CommandTimeout),
                                                       Dependency.OnValue(dependencyType: m.DataType.GetType(), value: m.DataType))
                                            .LifestyleSingleton());
            });
        }
    }
}