using System.Collections.Generic;

namespace FS.Core.LinkTrack;

public class ExceptionDetail
{
    /// <summary>
    /// 异常所属的方法完整签名
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// 方法入参值
    /// </summary>
    public Dictionary<string, string> MethodParams { get; set; }
    
    /// <summary>
    /// 发生时间
    /// </summary>
    public long CreateAt { get; set; }
    
    /// <summary>
    /// 异常类型
    /// </summary>
    public string ExceptionTypeName { get; set; }
    
    /// <summary>
    /// 异常类型
    /// </summary>
    public string ExceptionMessage { get; set; }
    
    /// <summary>
    /// 调用堆栈
    /// </summary>
    public List<string> CallStacks { get; set; }
}