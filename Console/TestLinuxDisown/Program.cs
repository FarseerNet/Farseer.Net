using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestLinuxDisown
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var log = $"{DateTime.Now:HH:mm:ss}";
                    //Console.WriteLine(log);

                    System.IO.File.AppendAllText(AppContext.BaseDirectory + "test.log", log);
                    Thread.Sleep(1000);
                }
            });
            Thread.Sleep(-1);
        }
    }
}