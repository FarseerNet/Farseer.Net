using System;
using System.Reflection;
using Farseer.Net.Cache.Test.Repository;
using FS.Cache;
using FS.Core.Abstract.Cache;
using FS.DI;
using FS.Modules;

namespace Farseer.Net.Cache.Test
{
    /// <summary>
    ///     缓存测试模块
    /// </summary>
    [DependsOn(typeof(CacheModule))]
    public class CacheTestModule : FarseerModule
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
            cacheServices.SetProfilesInMemory<UserPO, int>("user", o => o.Id, TimeSpan.FromSeconds(10)); //"default", 
        }
    }
}