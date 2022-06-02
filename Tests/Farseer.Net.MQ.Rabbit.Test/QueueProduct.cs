using System.Threading;
using Farseer.Net.MQ.Rabbit.Test.Consumer;
using FS;
using FS.DI;
using FS.MQ.Rabbit;
using NUnit.Framework;

namespace Farseer.Net.MQ.Rabbit.Test;

public class QueueProduct : BaseTests
{
    [Test]
    public void Send()
    {
        // 发送数据
        var rabbitProduct = IocManager.GetService<IRabbitManager>("UnitTest").Product;

        var result = rabbitProduct.Send(FarseerApplication.AppId.ToString());
        Assert.IsTrue(result);
        Thread.Sleep(1000);

        Assert.AreEqual(FarseerApplication.AppId, UnitTestConsumer.ID);
    }
}