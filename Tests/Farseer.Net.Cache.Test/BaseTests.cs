using FS;
using FS.Cache;

namespace Farseer.Net.Cache.Test;

public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<CacheTestModule>().Initialize();
    }
}