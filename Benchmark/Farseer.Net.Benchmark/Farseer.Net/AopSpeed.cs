using BenchmarkDotNet.Attributes;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace Farseer.Net.Benchmark.Farseer.Net;

public class AopSpeed : BaseBenchmark
{
    /// <summary>
    /// 正常调用
    /// </summary>
    [Benchmark(Baseline = true)]
    public void NormalCall()
    {
        GlobalCount.Count++;
    }


    /// <summary>
    /// 正常调用
    /// </summary>
    [Benchmark, HelloWorld]
    public void AopCall()
    {
    }

    /// <summary>
    /// 方法拦截测试
    /// </summary>
    [PSerializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class HelloWorldAttribute : MethodInterceptionAspect
    {
        /// <summary>
        /// 方法执行拦截
        /// </summary>
        /// <param name="args"></param>
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            // 处理(执行被拦截的方法)
            args.Proceed();
            GlobalCount.Count++;
        }
    }
    
    public class GlobalCount
    {
        public static int Count;
    }
}