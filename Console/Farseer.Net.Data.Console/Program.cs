using System.Threading.Tasks;
using FS;

namespace Farseer.Net.Data.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            FarseerBootstrapper.Create<StartupModule>().Initialize();

            var lst = new UserCoinsAgent().ToCreditList();
            var a = 1;
            a++;
        }
    }
}