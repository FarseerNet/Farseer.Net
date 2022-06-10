using System;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using Microsoft.Extensions.Logging;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace FS.Core.AOP
{
    /// <summary>
    /// 无限循环调用方法
    /// </summary>
    [PSerializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class WhileTrueAttribute : MethodInterceptionAspect
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private TimeSpan _sleepTimeSpan;

        /// <summary>
        /// 无限循环调用方法
        /// </summary>
        /// <param name="sleepTimeSpan">休眠时间</param>
        public WhileTrueAttribute(TimeSpan sleepTimeSpan)
        {
            _sleepTimeSpan = sleepTimeSpan;
        }

        /// <summary>
        /// 无限循环调用方法
        /// </summary>
        /// <param name="_millisecondsTimeout">休眠时间</param>
        public WhileTrueAttribute(int _millisecondsTimeout)
        {
            _sleepTimeSpan = TimeSpan.FromMilliseconds(_millisecondsTimeout);
        }

        /// <summary>
        /// 方法执行拦截
        /// </summary>
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            while (true)
            {
                try
                {
                    args.Proceed();
                }
                catch (System.Exception e)
                {
                    IocManager.Instance.Logger<WhileTrueAttribute>().LogError(e, e.Message);
                }
                Thread.Sleep(_sleepTimeSpan);
            }
        }

        public override async Task OnInvokeAsync(MethodInterceptionArgs args)
        {
            while (true)
            {
                try
                {
                    await args.ProceedAsync();
                }
                catch (System.Exception e)
                {
                    IocManager.Instance.Logger<WhileTrueAttribute>().LogError(e, e.Message);
                }
                await Task.Delay(_sleepTimeSpan);
            }
        }
    }
}