using System;
using System.Collections.Generic;
using Collections.Pooled;
using Nest;

namespace FS.LinkTrack.Dal;

public class ExceptionDetailPO : IDisposable
{
    /// <summary>
    ///     应用Id
    /// </summary>
    public long AppId { get; set; }

    /// <summary>
    ///     应用Ip
    /// </summary>
    [Keyword]
    public string AppIp { get; set; }
    /// <summary>long
    ///     应用名称
    /// </summary>
    [Keyword]
    public string AppName { get; set; }

    /// <summary>
    ///     上下文ID
    /// </summary>
    [Keyword]
    public string ContextId { get; set; }

    /// <summary>
    /// 异常所属的方法完整签名
    /// </summary>
    [Text]
    public string Method { get; set; }

    /// <summary>
    /// 方法入参值
    /// </summary>
    [Flattened]
    public PooledDictionary<string, string> MethodParams { get; set; }

    /// <summary>
    /// 发生时间
    /// </summary>
    [Date]
    public long CreateAt { get; set; }

    /// <summary>
    /// 异常类型
    /// </summary>
    [Keyword]
    public string ExceptionTypeName { get; set; }

    /// <summary>
    /// 异常类型
    /// </summary>
    [Text]
    public string ExceptionMessage { get; set; }

    /// <summary>
    /// 调用堆栈
    /// </summary>
    [Flattened]
    public PooledList<string> CallStacks { get; set; }


    public void Dispose()
    {
        MethodParams?.Dispose();
        CallStacks?.Dispose();
    }
}