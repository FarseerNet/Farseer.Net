using FS.Data;
using FS.Data.Abstract;
using FS.DI;

namespace Farseer.Net.DataDemo;

/// <summary>
/// 元信息上下文
/// </summary>
public class MysqlContext : DbContext<MysqlContext>
{
    public MysqlContext() : base(null)
    {
    }

    public TableSet<UserPO> User { get; set; }

    /// <summary>
    /// 这里可以设置表名
    /// </summary>
    protected override void CreateModelInit()
    {
        User.SetName("user");
    }

    protected override IDatabaseConnection SplitDatabase()
    {
        return IocManager.GetService<IDatabaseConnection>(name: $"dbConnection_test");
    }
}