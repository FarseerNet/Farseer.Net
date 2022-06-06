using System;

namespace FS.Cache.Attribute;

/// <summary>
/// 缓存删除的键
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class CacheIdAttribute : System.Attribute
{

}