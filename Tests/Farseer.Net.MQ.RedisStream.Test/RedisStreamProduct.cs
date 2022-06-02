using System.Threading;
using Farseer.Net.MQ.RedisStream.Test.Consumer;
using FS;
using FS.Core.Abstract.MQ.RedisStream;
using FS.DI;
using NUnit.Framework;

namespace Farseer.Net.MQ.RedisStream.Test;

public class RedisStreamProduct : BaseTests
{
    [Test]
    public void Send()
    {
        // 发送数据
        var redisStreamProduct = IocManager.GetService<IRedisStreamProduct>("UnitTest");
        var result = redisStreamProduct.Send(FarseerApplication.AppId.ToString());
        Assert.IsTrue(result);
        Thread.Sleep(1000);

        Assert.AreEqual(FarseerApplication.AppId, UnitTestConsumer.ID);
    }
}