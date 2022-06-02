using FS;
using FS.EventBus;

namespace Farseer.Net.EventBus.Test;

public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<EventBusModule>().Initialize();
    }
}