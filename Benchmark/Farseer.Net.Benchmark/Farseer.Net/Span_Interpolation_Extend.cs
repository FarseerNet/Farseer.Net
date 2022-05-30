using BenchmarkDotNet.Attributes;
using FS.Extends;

namespace Farseer.Net.Benchmark.Farseer.Net;

/*
|               Method |     Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|--------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|
| String_Interpolation | 32.94 ns | 0.752 ns | 2.120 ns |  1.00 |    0.00 | 0.0395 |     248 B |
|   Span_Interpolation | 80.23 ns | 2.527 ns | 7.252 ns |  2.44 |    0.26 | 0.0393 |     248 B |
*/
public class Span_Interpolation_Extend : AbsBenchmark
{
    [Benchmark(Baseline = true)]
    public string String_Interpolation()
    {
        var val = "Farseer.Net";
        return $"性能优化就是如何在保证处理相同数量的请求情况下占用更少的资源。{val}而这个资源一般就是CPU或者内存。当然还有操作系统IO句柄、网络流量、磁盘占用等等。但是绝大多数时候，我们就是在降低CPU和内存的占用率。";
    }

    [Benchmark()]
    public string Span_Interpolation()
    {
        var val = "Farseer.Net".AsSpan();
        return $"性能优化就是如何在保证处理相同数量的请求情况下占用更少的资源。{val}而这个资源一般就是CPU或者内存。当然还有操作系统IO句柄、网络流量、磁盘占用等等。但是绝大多数时候，我们就是在降低CPU和内存的占用率。";
    }
}