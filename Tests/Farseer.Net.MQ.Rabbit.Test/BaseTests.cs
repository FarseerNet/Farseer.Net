using FS;
using FS.MQ.Rabbit;

namespace Farseer.Net.MQ.Rabbit.Test;

public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<RabbitModule>().Initialize();
    }
}