using FS;
using FS.Cache.Redis;

namespace Farseer.Net.Cache.Redis.Test;

public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<RedisModule>().Initialize();
    }
}