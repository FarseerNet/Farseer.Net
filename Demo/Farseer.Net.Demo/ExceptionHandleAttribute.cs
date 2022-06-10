using System;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace Farseer.Net.Demo
{
    /// <summary>
    /// 异常处理
    /// </summary>
    [PSerializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class ExceptionHandleAttribute : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            // 设置流程行为（继续执行，不抛出）
            args.FlowBehavior = FlowBehavior.Continue;

            Console.WriteLine($"发生异常:{args.Exception.GetType().FullName} ----> {args.Exception.Message}");
        }
    }
}