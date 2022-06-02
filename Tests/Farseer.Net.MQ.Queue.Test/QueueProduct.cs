using System.Threading;
using Farseer.Net.MQ.Queue.Test.Consumer;
using FS;
using FS.Core.Abstract.MQ.Queue;
using FS.DI;
using NUnit.Framework;

namespace Farseer.Net.MQ.Queue.Test;

public class QueueProduct : BaseTests
{
    [Test]
    public void Send()
    {
        // 发送数据
        IocManager.GetService<IQueueProduct>("unit_test").Send(FarseerApplication.AppId);
        
        Thread.Sleep(500);
        
        Assert.AreEqual(FarseerApplication.AppId,UnitTestConsumer.ID);
    }
}