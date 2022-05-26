using FS.Core.Data;

namespace FS.Core.Repository;

public interface IRepository
{
    /// <summary>
    /// 创建事务
    /// </summary>
    ITransaction BeginTransaction();
}