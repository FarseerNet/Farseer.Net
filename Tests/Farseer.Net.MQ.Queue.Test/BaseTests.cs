using FS;
using FS.MQ.Queue;

namespace Farseer.Net.MQ.Queue.Test;

public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<QueueModule>().Initialize();
    }
}