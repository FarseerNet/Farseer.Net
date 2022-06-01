using FS;
using FS.Data;
using NUnit.Framework;

namespace Farseer.Net.Data.Test;

public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<DataModule>().Initialize();
    }
}