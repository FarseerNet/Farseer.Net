using System.Threading;
using Farseer.Net.MQ.RocketMQ.Console;
using FS;

namespace Farseer.Net.MQ.RocketMQ.Http.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FarseerBootstrapper.Create<StartupModule>().Initialize();

            System.Console.WriteLine("请输入：1）发送；2）消费");
            switch (System.Console.ReadLine())
            {
                case "2":
                {
                    System.Console.Title = "消费";
                    TestRocketMQ.Consumer(); break;
                }
                case "1":
                {
                    System.Console.Title = "发送";
                    TestRocketMQ.SendMessage(); break;
                }
            }

            Thread.Sleep(-1);
        }
    }
}