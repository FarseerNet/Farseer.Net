using System.Threading;
using FS;

namespace Farseer.Net.MQ.RocketMQ.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FarseerBoot.Run<StartupModule>().Initialize();

            System.Console.Write("请输入：配置节点名称，直接回车，为默认配置：");
            var configName = System.Console.ReadLine();
            if (string.IsNullOrWhiteSpace(configName)) configName = "test";
            System.Console.WriteLine(configName);

            System.Console.WriteLine("请输入：1）发送；2）消费");
            switch (System.Console.ReadLine())
            {
                case "2":
                    {
                        System.Console.Title = "消费";
                        TestRocketMQ.Consumer(configName); break;
                    }
                case "1":
                    {
                        System.Console.Title = "发送";
                        TestRocketMQ.SendMessage(configName); break;
                    }
            }

            Thread.Sleep(-1);
        }
    }
}