using FS.Modules;
using FS.MQ.RedisStream;

namespace Farseer.Net.MQ.RedisStreamDemo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn(typeof(RedisStreamModule))]
    public class StartupModule : FarseerModule
    {
    }
}