using System;

// ReSharper disable once CheckNamespace
namespace FS.Tasks;

/// <summary>
///     JOB配置
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
public class JobAttribute : Attribute
{
    /// <summary>
    ///     是否启用（默认为true）
    /// </summary>
    public bool Enable { get; set; } = true;
    /// <summary>
    ///     是否启动后立即执行（默认为true）
    /// </summary>
    public bool StartupExecute { get; set; } = true;

    /// <summary>
    ///     执行间隔（0：只执行一次，>0：ms间隔执行一次）
    /// </summary>
    public long Interval { get; set; }
}