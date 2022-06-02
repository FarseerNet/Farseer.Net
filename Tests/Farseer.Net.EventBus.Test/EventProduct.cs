using FS;
using FS.Core.Abstract.EventBus;
using FS.DI;
using NUnit.Framework;

namespace Farseer.Net.EventBus.Test;

public class EventProduct : BaseTests
{
    [Test]
    public void Send()
    {
        // 发送数据
        IocManager.GetService<IEventProduct>("unit_test").SendSync(this, FarseerApplication.AppId);

        Assert.AreEqual(FarseerApplication.AppId, UnitTestEvent.ID);
    }
}