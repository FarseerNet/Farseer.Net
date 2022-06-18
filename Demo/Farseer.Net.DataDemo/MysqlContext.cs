using FS.Data;
using FS.Data.Abstract;
using FS.DI;

namespace Farseer.Net.DataDemo;

/// <summary>
/// 数据库上下文
/// </summary>
public class MysqlContext : DbContext<MysqlContext>
{
    public MysqlContext() : base("dbConnection_test") { }

    public TableSet<UserPO> User { get; set; }

    /// <summary>
    /// 这里可以设置表名
    /// </summary>
    protected override void CreateModelInit()
    {
        User.SetName("user");
    }

    // /// <summary>
    // /// 也可以在这里动态设置数据库配置
    // /// </summary>
    // protected override IDatabaseConnection SplitDatabase()
    // {
    //     return IocManager.GetService<IDatabaseConnection>(name: $"dbConnection_test");
    // }
}