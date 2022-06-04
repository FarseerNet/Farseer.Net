using Farseer.Net.Data.Test.Repository;
using FS;
using FS.Core.Abstract.Data;
using FS.Core.AOP.Data;
using FS.Data;
using NUnit.Framework;

namespace Farseer.Net.Data.Test;

public class TransactionTest : BaseTests
{
    [Test]
    [Transaction(typeof(MysqlContext))]
    public void AopTransaction()
    {
        // 添加
        MysqlContext.Data.User.Insert(new UserPO()
        {
            Name = FarseerApplication.AppId.ToString()
        });

        // 删除
        MysqlContext.Data.User.Where(o => o.Name == FarseerApplication.AppId.ToString()).Delete();
    }
}