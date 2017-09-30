//using System;
//using System.Diagnostics;
//using System.Reflection;
//using Castle.Core.Logging;
//using NLog.LayoutRenderers;
//using ZTO.Platform.Core.Context;
//using ZTO.Platform.DI;
//using NullLogger = Castle.Core.Logging.NullLogger;

//namespace ZTO.Platform.Log
//{
//    public class NLogClient
//    {
//        private readonly IExtendedLoggerFactory _factory = null;
//        /// <summary>
//        /// 获取针对调用类的日志对象
//        /// </summary>
//        /// <returns></returns>
//        public ILogger GetCurrentClassLogger()
//        {
//            return _factory?.Create(GetClassFullName())?? NullLogger.Instance;
//        }
//        /// <summary>
//        /// 获取调用GetCurrentClassLogger 方法的类的FullName
//        /// </summary>
//        /// <returns></returns>
//        private string GetClassFullName()
//        {
//            string className;
//            Type declaringType;
//            int framesToSkip = 2;

//            do
//            {
//#if SILVERLIGHT
//                StackFrame frame = new StackTrace().GetFrame(framesToSkip);
//#else
//                StackFrame frame = new StackFrame(framesToSkip, false);
//#endif
//                MethodBase method = frame.GetMethod();
//                declaringType = method.DeclaringType;
//                if (declaringType == null)
//                {
//                    className = method.Name;
//                    break;
//                }

//                framesToSkip++;
//                className = declaringType.FullName;
//            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

//            return className;
//        }
//        public NLogClient()
//        {
//            RegisterTraceId();
//            if(IocManager.Instance.IsRegistered<IExtendedLoggerFactory>())
//                _factory =  IocManager.Instance.Resolve<IExtendedLoggerFactory>();
//        }
//        /// <summary>
//        /// Nlog的Layout中增加TraceId元素
//        /// </summary>
//        private void RegisterTraceId()
//        {
//            var traceId = TraceIdContext.Current?.TraceId ?? "traceId";
//            LayoutRenderer.Register("traceid", (logEvent) => traceId);
//            LayoutRenderer.Register("MyDateTime", (logEvent) => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
//        }
//    }
//}
