using System;
using System.Linq;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace Farseer.Net.Demo
{
    /// <summary>
    /// 方法拦截测试
    /// </summary>
    [PSerializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class HowToUseAttribute : MethodInterceptionAspect
    {
        /// <summary>
        /// 方法执行拦截
        /// </summary>
        /// <param name="args"></param>
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var methodBase = args.Method;

            // 如果是 .NET Framework 这里的System.Reflection.MethodInfo 应该替换为 System.Reflection.RuntimeMethodInfo
            var returnType = ((System.Reflection.MethodInfo)methodBase).ReturnType.FullName;

            // 方法形式参数列表字符
            var paramListString = methodBase.GetParameters().Aggregate(string.Empty,
                                                                       (current, parameter) => current + $"{parameter.ParameterType.FullName} {parameter.Name}, ").Trim(',', ' ');

            // 方法签名
            // var signatures =  $"{returnType} {methodBase.Name}（{paramListString}）";
            var signatures = methodBase.ToString();

            Console.WriteLine($"被拦截的方法签名：{signatures}");

            // 方法实际参数列表字符
            var argsString = args.Arguments
                                 .Aggregate(string.Empty, (current, p) => current + $"{p.GetType().FullName} ---> 值：{p}, ").Trim(',', ' ');

            Console.WriteLine($"被拦截的方法输入参数：{argsString}");

            // 处理(执行被拦截的方法)
            args.Proceed();

            // 异步执行
            // args.ProceedAsync();

            var returnValue = args.ReturnValue;

            Console.WriteLine($"方法返回值：{returnValue}");
        }
    }

}