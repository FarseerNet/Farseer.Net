using FS;

namespace Farseer.Net.DataDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();

            var lst = new UserCoinsAgent().ToCreditList();
            var a   = 1;
            a++;
        }
    }
}