using FS.DI;

namespace Farseer.net.AutoMapper.Test
{
    public interface ITest: ITransientDependency
    {
        void Console();
    }
}