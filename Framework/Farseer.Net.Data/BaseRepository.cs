using FS.Core.Abstract.Data;
using FS.Core.DomainDriven.Repository;

namespace FS.Data;

/// <summary>
/// 仓储层的基类
/// </summary>
public abstract class BaseRepository<TDbContext> : IRepository where TDbContext : DbContext<TDbContext>, new()
{
    /// <summary>
    /// 创建事务
    /// </summary>
    public ITransaction BeginTransaction() => new TDbContext();
}