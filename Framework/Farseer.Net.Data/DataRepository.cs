using FS.Core.Data;
using FS.Core.Repository;

namespace FS.Data;

/// <summary>
/// 仓储层的基类
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public abstract class DataRepository<TDbContext> : IRepository where TDbContext : DbContext<TDbContext>, new()
{
    /// <summary>
    /// 创建事务
    /// </summary>
    public ITransaction BeginTransaction() => new TDbContext();
}