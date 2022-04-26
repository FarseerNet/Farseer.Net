using FS.Modules;
using FS.Tasks;

namespace Farseer.Net.TaskDemo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn(typeof(TaskModule))] // 依赖Job模块
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}