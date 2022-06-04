using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Collections.Pooled;
using FS.Extends;

namespace FS.Core.LinkTrack
{
    public sealed class LinkTrackDetail
    {
        const string farseerNet = "Farseer.Net";
        const string callback   = "Callback";
        const string moveNext   = "MoveNext";

        internal StackTrace   _stackTrace;

        /// <summary>
        ///     调用栈
        /// </summary>
        public CallStackTrace CallStackTrace { get; set; }

        /// <summary>
        ///     调用方法
        /// </summary>
        public string CallMethod { get; set; }

        /// <summary>
        ///     调用类型
        /// </summary>
        public EumCallType CallType { get; set; }

        /// <summary>
        ///     埋点数据
        /// </summary>
        public PooledDictionary<string, string> Data { get; set; } = new();

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
            // https://docs.microsoft.com/zh-cn/dotnet/api/system.runtime.compilerservices.callerlinenumberattribute?redirectedfrom=MSDN&view=net-6.0
            foreach (var stackFrame in _stackTrace.GetFrames())
            {
                var fileLineNumber = stackFrame.GetFileLineNumber();
                var methodBase     = stackFrame.GetMethod();

                if (fileLineNumber == 0 || methodBase.IsAssembly || methodBase.Module.Name.Contains(value: farseerNet) || methodBase.Name.Contains(value: callback)) continue;

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
                var methodParams   = parameterInfos.Length > 0 ? parameterInfos.ToPooledDictionary(keySelector: o => o.Name, o => o.ParameterType.Name) : new PooledDictionary<string, string>();

                CallStackTrace = new CallStackTrace
                {
                    ReturnType     = returnType,
                    CallMethod     = $"{methodBase.DeclaringType?.Name}.{methodBase.Name.Replace(oldValue: ".", newValue: "")}",
                    MethodParams   = methodParams,
                    FileLineNumber = fileLineNumber,
                    FileName       = stackFrame.GetFileName()
                };
                if (!string.IsNullOrWhiteSpace(CallStackTrace.FileName))
                {
                    CallStackTrace.FileName = new FileInfo(CallStackTrace.FileName).Name;
                }
                break;
            }
        }

        /// <summary>
        /// 设置SQL入参
        /// </summary>
        /// <param name="param"></param>
        public void SetDbParam(IEnumerable<DbParameter> param)
        {
            if (param == null || !param.Any()) return;
            foreach (var sqlParam in param)
            {
                Data["Sql"] = Data["Sql"].Replace(sqlParam.ParameterName, $"\"{sqlParam.Value}\"");
            }
        }
    }
}