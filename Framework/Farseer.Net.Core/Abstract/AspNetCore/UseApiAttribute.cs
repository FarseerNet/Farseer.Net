using System;

namespace FS.Core.Abstract.AspNetCore;

[AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
public class UseApiAttribute : System.Attribute
{
    /// <summary>
    /// 区域
    /// </summary>
    public string Area { get; set; } 
}