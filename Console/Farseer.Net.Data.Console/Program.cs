using System.Threading.Tasks;
using FS;

namespace Farseer.Net.Data.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            FarseerBootstrapper.Create<StartupModule>().Initialize();
            using (var db = new TestContext())
            {
                db.SaveChanges();
            }
            var lst = new UserCoinsAgent().ToCreditList();
            var a = 1;
            a++;
        }
    }
}