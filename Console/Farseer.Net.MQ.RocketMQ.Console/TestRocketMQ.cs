// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-27 18:38
// ********************************************

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Farseer.Net.DI;
using Farseer.Net.MQ.RocketMQ.SDK;

namespace Farseer.Net.MQ.RocketMQ.Console
{
    public class TestRocketMQ
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        public static void SendMessage(string readLine)
        {
            var rocket = IocManager.Instance.Resolve<IRocketMQManager>(readLine);
            try
            {
                var startAt = DateTime.Now;
                rocket.Product.Start();
                IocManager.Instance.Resolve<IRocketMQManager>(readLine).Product.Start();
                Parallel.For(0, 10000000, new ParallelOptions { MaxDegreeOfParallelism = 64 }, i =>
                {
                    var msgID = rocket.Product.Send($"PID:{Process.GetCurrentProcess().Id} index:{i},Time：{DateTime.Now}", "testTag").getMessageId();
                    System.Console.WriteLine(msgID);
                });

                // 输出时间
                System.Console.WriteLine((DateTime.Now - startAt).TotalMilliseconds);
            }
            finally { rocket.Product.Close(); }
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        public static void Consumer(string readLine)
        {
            var rocket = IocManager.Instance.Resolve<IRocketMQManager>(readLine);
            try
            {
                rocket.Consumer.Start(SaveDbListener.Instance, "testTag");
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
    public class SaveDbListener : MessageListener
    {
        private SaveDbListener() { }
        public static readonly SaveDbListener Instance = new SaveDbListener();

        public override SDK.Action consume(Message message, ConsumeContext context)
        {
            System.Console.Write("开始：");
            System.Console.WriteLine(message.getMsgBody());
            return SDK.Action.CommitMessage;
        }
    }
}