using FS;
using FS.Core.Abstract.Data;
using FS.Core.AOP.Data;

namespace Farseer.Net.DataDemo;

public class TransactionTest
{
    /// <summary>
    /// 开启事务（指定上下文类型）
    /// </summary>
    [Transaction(typeof(MysqlContext))]
    public void AopTransaction()
    {
        MysqlContext.Data.User.Insert(new UserPO { Name = FarseerApplication.AppId.ToString() }); // 添加
        MysqlContext.Data.User.Where(o => o.Name == FarseerApplication.AppId.ToString()).Delete(); // 删除
    }

    /// <summary>
    /// 开启事务（指定数据库配置名称）
    /// </summary>
    [Transaction("test")]
    public void AopTransactionByName()
    {
        MysqlContext.Data.User.Insert(new UserPO { Name = FarseerApplication.AppId.ToString() }); // 添加
        MysqlContext.Data.User.Where(o => o.Name == FarseerApplication.AppId.ToString()).Delete(); // 删除
    }

    /// <summary>
    /// 开启事务（手动实例化）
    /// </summary>
    public void NormalTransaction()
    {
        using (var db = new MysqlContext())
        {
            db.User.Insert(new UserPO { Name = FarseerApplication.AppId.ToString() }); // 添加
            MysqlContext.Data.User.Where(o => o.Name == FarseerApplication.AppId.ToString()).Delete(); // 删除
            db.SaveChanges();
        }
    }
}