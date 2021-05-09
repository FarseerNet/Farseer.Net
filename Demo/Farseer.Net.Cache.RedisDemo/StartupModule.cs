using FS.Cache.Redis;
using FS.Modules;

namespace Farseer.Net.Cache.RedisDemo
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(RedisModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
        }
    }
}