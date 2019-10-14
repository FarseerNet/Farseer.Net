using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.MQ.RocketMQ;
using Action = FS.MQ.RocketMQ.SDK.Action;

namespace Farseer.Net.MQ.RocketMQ.Console
{
    public class TestRocketMQ
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        public static void SendMessage()
        {
            var rocket = IocManager.Instance.Resolve<IRocketMQManager>("test");
            try
            {
                var startAt = DateTime.Now;
                rocket.HttpProduct.Start();
                //Parallel.For(0, 10000000, new ParallelOptions { MaxDegreeOfParallelism = 64 }, i =>
                {
                    var result = rocket.HttpProduct.Send($"PID:{Process.GetCurrentProcess().Id} index:,Time：{DateTime.Now}", "testTag");
                    System.Console.WriteLine(result.Id);
                }//);

                // 输出时间
                System.Console.WriteLine((DateTime.Now - startAt).TotalMilliseconds);
            }
            finally { rocket.HttpProduct.Close(); }
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        public static void Consumer()
        {
            var rocket = IocManager.Instance.Resolve<IRocketMQManager>("test");
            try
            {
                rocket.HttpConsumer.Start(SaveDbListener.Instance, "testTag");
                System.Console.WriteLine("开始消费：StartPushConsumer");
            }
            finally
            {
                Thread.Sleep(20000);
                System.Console.WriteLine("时间已到，我要回收了");
                GC.Collect();
            }
        }
    }

    /// <summary>
    ///     消息监听消费
    /// </summary>
    public class SaveDbListener : HttpMessageListener
    {
        private SaveDbListener() { }
        public static readonly SaveDbListener Instance = new SaveDbListener();

        public override Action Consume(FS.MQ.RocketMQ.SDK.Http.Model.Message message)
        {
            System.Console.Write("开始：");
            System.Console.WriteLine(message.Body);
            return Action.CommitMessage;
        }
    }
}