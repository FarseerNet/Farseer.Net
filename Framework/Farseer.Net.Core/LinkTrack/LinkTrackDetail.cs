using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FS.Extends;

namespace FS.Core.LinkTrack
{
    public sealed class LinkTrackDetail
    {
        const    string       farseerNet = "Farseer.Net";
        const    string       callback   = "Callback";
        const    string       moveNext   = "MoveNext";
        
        internal StackFrame[] _lstFrames;
        internal StackTrace   _stackTrace;

        /// <summary>
        ///     调用栈
        /// </summary>
        public List<CallStackTrace> CallStackTraceList { get; set; }

        /// <summary>
        ///     调用方法
        /// </summary>
        public string CallMethod { get; set; }

        /// <summary>
        ///     调用类型
        /// </summary>
        public EumCallType CallType { get; set; }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        public DbLinkTrackDetail DbLinkTrackDetail { get; set; }

        /// <summary>
        ///     埋点数据
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        /// <summary>
        ///     调用开始时间戳
        /// </summary>
        public long StartTs { get; set; } = DateTime.Now.ToTimestamps();

        /// <summary>
        ///     调用停止时间戳
        /// </summary>
        public long EndTs { get; set; }

        /// <summary>
        ///     总共使用时间毫秒
        /// </summary>
        public long UseTs => EndTs > StartTs ? EndTs - StartTs : 0;

        /// <summary>
        ///     是否执行异常
        /// </summary>
        public bool IsException { get; set; }

        /// <summary>
        ///     异常信息
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        ///     追踪调用堆栈
        /// </summary>
        public void SetCallStackTrace()
        {
            if (_stackTrace == null) return;
            CallStackTraceList = new List<CallStackTrace>();
            foreach (var stackFrame in _stackTrace.GetFrames())
            {
                if (CallStackTraceList.Count > 3) break; // 最多存3条
                var fileLineNumber = stackFrame.GetFileLineNumber();
                var methodBase     = stackFrame.GetMethod();

                if (fileLineNumber           == 0 || methodBase.IsAssembly || methodBase.Module.Name.Contains(value: farseerNet) || methodBase.Name.Contains(value: callback)) continue;
                
                if (methodBase.DeclaringType != null && methodBase.DeclaringType.Name.Contains(value: ">d") && methodBase.Name == moveNext) continue;

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
                                lstType.Push(item: $"{curType.Name.Replace(oldValue: "`1", newValue: "")}<>");
                                curType = curType.GenericTypeArguments[0];
                                continue;
                            }

                            lstType.Push(item: curType.Name);
                            break;
                        }

                        // 依次弹出元素类型
                        returnType = lstType.Pop();
                        while (lstType.Count > 0) returnType = lstType.Pop().Replace(oldValue: "<>", newValue: $"<{returnType}>");
                    }
                    else
                        returnType = methodInfo.ReturnType.Name;
                }

                // 方法入参
                var parameterInfos = methodBase.GetParameters();
                var methodParams   = parameterInfos.Length > 0 ? parameterInfos.ToDictionary(keySelector: o => o.Name, elementSelector: o => o.ParameterType.Name) : new Dictionary<string, string>();

                CallStackTraceList.Add(item: new CallStackTrace
                {
                    ReturnType     = returnType,
                    CallMethod     = $"{methodBase.DeclaringType?.Name}.{methodBase.Name.Replace(oldValue: ".", newValue: "")}",
                    MethodParams   = methodParams,
                    FileLineNumber = fileLineNumber,
                    FileName       = stackFrame.GetFileName()
                });
            }
        }
    }
}