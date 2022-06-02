using FS;
using FS.Tasks;

namespace Farseer.Net.Tasks.Test;

[Tasks]
public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<TaskModule>().Initialize();
    }
}