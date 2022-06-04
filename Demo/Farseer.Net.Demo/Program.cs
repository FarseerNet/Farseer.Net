using System;
using System.Linq;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace NetCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Calc(5, 6);
            Console.WriteLine($"计算结果：{result}");
            Console.WriteLine(">>>>>>>>>>>>>>方法拦截测试完毕\r\n");


            PropertyTest = -1;
            Console.WriteLine(">>>>>>>>>>>>>>属性拦截测试(setter)完毕\r\n");


            var x = PropertyTest;
            Console.WriteLine(">>>>>>>>>>>>>>属性拦截测试(getter)完毕\r\n");

            Console.ReadKey();
        }


        /// <summary>
        /// 方法拦截测试 + 异常处理
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [HowToUse, ExceptionHandle]
        private static int Calc(int x, int y)
        {
            int a = 1;
            int b = 0;
            int c = a / b;

            return x + y;
        }

        private static int _propertyTest;

        /// <summary>
        /// 属性拦截测试
        /// 注：可以标记在整个属性上，也可以分别单独标记在 【getter】 或者 【setter】 上
        /// </summary>
        [HowToUse, ExceptionHandle]
        private static int PropertyTest
        {
            get
            {
                return _propertyTest;
            }
            // [HowToUse]
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"属性值必须大于0");
                }

                _propertyTest = value;
            }
        }
    }
}

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