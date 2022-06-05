using System;

namespace FS.Core.Abstract.Data;

public interface IDbExecutor : IDisposable
{
    /// <summary>
    ///     连接字符串
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    ///     是否开启事务
    /// </summary>
    bool IsTransaction { get; }
}