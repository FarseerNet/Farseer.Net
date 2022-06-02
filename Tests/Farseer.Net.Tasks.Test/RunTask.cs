using System.Threading;
using FS;
using FS.Tasks;
using NUnit.Framework;

namespace Farseer.Net.Tasks.Test;

[Tasks] // 开启后，才能把JOB自动注册进来
public class RunTask : BaseTests
{
    [Test]
    public void JobRun()
    {
        Thread.Sleep(10000);
        Assert.AreEqual(FarseerApplication.AppId, UnitTestJob.ID);
        Assert.IsTrue(UnitTestJob.Interval >= 200);
        Assert.IsTrue(UnitTestJob.Interval < 230);
    }
}