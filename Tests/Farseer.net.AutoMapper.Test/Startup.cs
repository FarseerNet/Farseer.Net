using Castle.MicroKernel.Registration;
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
            IocManager.RegisterAssemblyByConvention(GetType());
            IocManager.Register(typeof(Test),typeof(Test));
        }
    }
}