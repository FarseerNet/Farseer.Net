using BenchmarkDotNet.Attributes;
using FS;
using FS.Data;

namespace Farseer.Net.Benchmark;

[MemoryDiagnoser]
public class BaseBenchmark
{
    [GlobalSetup]
    public void Setup()
    {
        FarseerApplication.Run<DataModule>().Initialize();
    }
}