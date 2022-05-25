using System;
using System.Data;

namespace FS.Data;

public interface ITransaction : IDisposable
{
    /// <summary>
    ///     保存修改
    /// </summary>
    void SaveChanges();
    /// <summary>
    ///     回滚事务
    /// </summary>
    void Rollback();
    /// <summary>
    ///     改变事务级别
    /// </summary>
    /// <param name="tranLevel"> 事务方式 </param>
    void ChangeTransaction(IsolationLevel tranLevel);
}