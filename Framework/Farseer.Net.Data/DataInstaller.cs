using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Collections.Pooled;
using FS.Data.Abstract;
using FS.Data.Client;
using FS.Data.Client.ClickHouse;
using FS.Data.Client.MySql;
using FS.Data.Client.PostgreSql;
using FS.Data.Client.SqLite;
using FS.Data.Configuration;
using FS.Data.Internal;
using FS.DI;
using FS.Extends;

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
            // 注册DbProvider
            container.Register(Component.For<AbsDbProvider>().Named(name: $"dbProvider_{eumDbType.MySql}").ImplementedBy<MySqlProvider>().LifestyleSingleton());
            container.Register(Component.For<AbsDbProvider>().Named(name: $"dbProvider_{eumDbType.ClickHouse}").ImplementedBy<ClickHouseProvider>().LifestyleSingleton());
            container.Register(Component.For<AbsDbProvider>().Named(name: $"dbProvider_{eumDbType.SQLite}").ImplementedBy<SqLiteProvider>().LifestyleSingleton());
            container.Register(Component.For<AbsDbProvider>().Named(name: $"dbProvider_{eumDbType.PostgreSql}").ImplementedBy<PostgreSqlProvider>().LifestyleSingleton());

            // 读取配置
            using var dbConfigs = DataConfigRoot.Get().ToPooledList();
            dbConfigs.ForEach(action: m =>
            {
                // 注册Db连接
                var dbConnectionString = IocManager.GetService<AbsDbProvider>($"dbProvider_{m.DataType}").ConnectionString.Create(server: m.Server, port: m.Port, userId: m.UserID, passWord: m.PassWord, catalog: m.Catalog, dataVer: m.DataVer, additional: m.Additional, connectTimeout: m.ConnectTimeout, poolMinSize: m.PoolMinSize, poolMaxSize: m.PoolMaxSize);
                container.Register(
                                   Component.For<IDatabaseConnection>()
                                            .Named(name: $"dbConnection_{m.Name}")
                                            .ImplementedBy<DatabaseConnection>()
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