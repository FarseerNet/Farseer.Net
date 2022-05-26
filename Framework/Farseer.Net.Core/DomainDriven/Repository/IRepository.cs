using FS.Core.Abstract.Data;

namespace FS.Core.DomainDriven.Repository;

public interface IRepository
{
    /// <summary>
    /// 创建事务
    /// </summary>
    ITransaction BeginTransaction();
}