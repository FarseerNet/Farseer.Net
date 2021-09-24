using FS.Mapper;
using FS.Modules;

namespace Farseer.net.AutoMapper.Test
{
    [DependsOn(typeof(MapperModule))]
    public class Startup : FarseerModule
    {
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
            IocManager.RegisterAssemblyByConvention(type: GetType());
            IocManager.Register(type: typeof(Test), impl: typeof(Test));
        }
    }
}