using BenchmarkDotNet.Attributes;
using FS;
using FS.Data;

namespace Farseer.Net.Benchmark;

[MemoryDiagnoser]
public class AbsBenchmark
{
    [GlobalSetup]
    public void Setup()
    {
        FarseerApplication.Run<DataModule>().Initialize();
    }
}