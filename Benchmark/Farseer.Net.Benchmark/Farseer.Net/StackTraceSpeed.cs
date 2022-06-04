using System.Diagnostics;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace Farseer.Net.Benchmark.Farseer.Net;

public class StackTraceSpeed : BaseBenchmark
{
    /// <summary>
    /// 正常调用
    /// </summary>
    [Benchmark(Baseline = true)]
    public void NormalCall()
    {
    }


    /// <summary>
    /// 正常调用
    /// </summary>
    [Benchmark, AopSpeed.HelloWorldAttribute]
    public MethodBase? StackCall()
    {
        return new StackTrace(true).GetFrame(1).GetMethod();

    }
}