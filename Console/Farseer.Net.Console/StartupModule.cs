using ZTO.Test.Commom;

namespace ZTO.Platform.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [Modules.DependsOn]
    public class StartupModule : ZTO.Platform.Modules.ZtoModule
    {
        public override void PreInitialize()
        {
		}

		public override void PostInitialize()
		{
		    var type = typeof(TestAutoRegister);
		    IocManager.RegisterAssemblyByConvention(this.GetType());
		}
	}
}
