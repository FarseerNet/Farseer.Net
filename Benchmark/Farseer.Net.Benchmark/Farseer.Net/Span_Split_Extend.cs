using BenchmarkDotNet.Attributes;
using Collections.Pooled;
using FS.Extends;

namespace Farseer.Net.Benchmark.Farseer.Net;

public class Span_Split_Extend : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    public string[] String_Split()
    {
        var str = "性能优化就是如何在保证处理相同数量的请求情况下占用更少的资源。而这个资源一般就是CPU或者内存。当然还有操作系统IO句柄、网络流量、磁盘占用等等。但是绝大多数时候，我们就是在降低CPU和内存的占用率。";
        return str.Split('。');
    }
    
    [Benchmark()]
    public PooledList<string> Span_Split()
    {
        var str = "性能优化就是如何在保证处理相同数量的请求情况下占用更少的资源。而这个资源一般就是CPU或者内存。当然还有操作系统IO句柄、网络流量、磁盘占用等等。但是绝大多数时候，我们就是在降低CPU和内存的占用率。";
        return str.AsSpan().Split("。");
    }
}