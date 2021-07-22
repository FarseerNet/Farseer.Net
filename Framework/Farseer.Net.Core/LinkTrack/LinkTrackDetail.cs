using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Castle.Core.Internal;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Core.LinkTrack
{
    public class LinkTrackDetail
    {
        public void SetCallStackTrace()
        {
            if (_stackTrace == null) return;
            CallStackTraceList = new List<CallStackTrace>();
            foreach (var stackFrame in _stackTrace.GetFrames())
            {
                var fileLineNumber = stackFrame.GetFileLineNumber();
                var methodBase     = stackFrame.GetMethod();

                if (fileLineNumber == 0 || methodBase.IsAssembly || methodBase.Module.Name.Contains("Farseer.Net") || methodBase.Name.Contains("Callback")) continue;
                if (methodBase.DeclaringType != null && methodBase.DeclaringType.Name.Contains(">d__") && methodBase.Name == "MoveNext") continue;

                // 方法返回类型
                var returnType = "";
                if (methodBase is MethodInfo methodInfo)
                {
                    // 需要判断是否为泛型类型，比如：List<>、Task<>
                    if (methodInfo.ReturnType.GenericTypeArguments.Length > 0)
                    {
                        var curType = methodInfo.ReturnType;
                        var lstType = new Stack<string>();
                        while (true)
                        {
                            if (curType.GenericTypeArguments.Length > 0)
                            {
                                lstType.Push($"{curType.Name.Replace("`1", "")}<>");
                                curType = curType.GenericTypeArguments[0];
                                continue;
                            }

                            lstType.Push(curType.Name);
                            break;
                        }

                        // 依次弹出元素类型
                        returnType = lstType.Pop();
                        while (lstType.Count > 0)
                        {
                            returnType = lstType.Pop().Replace("<>", $"<{returnType}>");
                        }
                    }
                    else returnType = methodInfo.ReturnType.Name;
                }

                // 方法入参
                var parameterInfos = methodBase.GetParameters();
                var methodParams   = parameterInfos.Length > 0 ? parameterInfos.ToDictionary(o => o.Name, o => o.ParameterType.Name) : new Dictionary<string, string>();

                CallStackTraceList.Add(new CallStackTrace()
                {
                    ReturnType     = returnType,
                    CallMethod     = $"{methodBase.DeclaringType?.Name}.{methodBase.Name.Replace(".", "")}",
                    MethodParams   = methodParams,
                    FileLineNumber = fileLineNumber,
                    FileName       = stackFrame.GetFileName()
                });
            }
        }

        internal StackFrame[] _lstFrames;

        internal StackTrace _stackTrace;

        /// <summary>
        /// 调用栈
        /// </summary>
        public List<CallStackTrace> CallStackTraceList { get; set; }

        /// <summary>
        /// 调用方法
        /// </summary>
        public string CallMethod { get; set; }

        /// <summary>
        /// 调用类型
        /// </summary>
        public EumCallType CallType { get; set; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public DbLinkTrackDetail DbLinkTrackDetail { get; set; }

        /// <summary>
        /// 埋点数据
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        /// <summary>
        /// 调用开始时间戳
        /// </summary>
        public long StartTs { get; set; } = DateTime.Now.ToTimestamps();

        /// <summary>
        /// 调用停止时间戳
        /// </summary>
        public long EndTs { get; set; }

        /// <summary>
        /// 总共使用时间毫秒
        /// </summary>
        public long UseTs => EndTs > StartTs ? EndTs - StartTs : 0;

        /// <summary>
        /// 是否执行异常
        /// </summary>
        public virtual bool IsException { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public virtual string ExceptionMessage { get; set; }
    }
}