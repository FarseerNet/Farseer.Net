using System;

// ReSharper disable once CheckNamespace
namespace FS.Tasks;

/// <summary>
///     标记Main方法启用消费
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
public class TasksAttribute : Attribute
{
    /// <summary>
    ///     是否启用（默认为true）
    /// </summary>
    public bool Enable { get; set; } = true;
}