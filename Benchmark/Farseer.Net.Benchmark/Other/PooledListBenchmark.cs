using BenchmarkDotNet.Attributes;
using Collections.Pooled;
using Farseer.Net.Benchmark.Data;

namespace Farseer.Net.Benchmark.Other;

// |                  Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Allocated |
// |------------------------ |----------:|----------:|----------:|------:|--------:|----------:|----------:|
// |              TestToList |  5.000 ms | 0.0277 ms | 0.0231 ms |  1.00 |    0.00 | 1289.0625 |  7,938 KB |
// |        TestToPooledList | 15.020 ms | 0.1979 ms | 0.1653 ms |  3.00 |    0.04 |   15.6250 |    125 KB |
// | TestToList_ToPooledList |  5.860 ms | 0.1007 ms | 0.0942 ms |  1.18 |    0.02 | 1296.8750 |  7,992 KB |
[MemoryDiagnoser]
public class PooledListBenchmark
{
    public List<UserPO> lst;
    [GlobalSetup]
    public void Setup()
    {
        lst = new();
        for (int i = 0; i < 1000; i++)
        {
            lst.Add(new UserPO() { Name = "123123" });
        }
    }
    
    [Benchmark(Baseline = true)]
    public void TestToList()
    {
        for (int i = 0; i < 1000; i++)
        {
            var lst2 = lst.Select(o => o.Name).ToList();
        }
    }

    [Benchmark()]
    public void TestToPooledList()
    {
        for (int i = 0; i < 1000; i++)
        {
            using var lst2 = lst.Select(o => o.Name).ToPooledList();
        }
    }

    [Benchmark()]
    public void TestToList_ToPooledList()
    {
        for (int i = 0; i < 1000; i++)
        {
            using var lst2 = lst.Select(o => o.Name).ToList().ToPooledList();
        }
    }
}