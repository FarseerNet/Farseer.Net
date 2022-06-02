using FS;
using FS.MQ.RedisStream;

namespace Farseer.Net.MQ.RedisStream.Test;

public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<RedisStreamModule>().Initialize();
    }
}