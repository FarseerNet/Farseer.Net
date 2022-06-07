using Farseer.Net.CacheDemo.Repository;
using FS.Cache;
using FS.Cache.Redis;
using FS.Modules;

namespace Farseer.Net.CacheDemo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn(typeof(RedisModule))]
    public class StartupModule : FarseerModule
    {
        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
        }

        public override void PostInitialize()
        {
            var cacheServices = IocManager.Resolve<ICacheServices>();
            //cacheServices.SetProfilesInMemory<UserPO, int>("user", o => o.Id, TimeSpan.FromSeconds(10)); //"default", 
            cacheServices.SetProfilesInMemory<UserPO>("user", TimeSpan.FromSeconds(10));                 //"default", 
        }
    }
}