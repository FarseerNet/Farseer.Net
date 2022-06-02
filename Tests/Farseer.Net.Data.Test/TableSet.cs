using System;
using System.Linq;
using System.Threading.Tasks;
using Farseer.Net.Data.Test.Repository;
using FS;
using NUnit.Framework;

namespace Farseer.Net.Data.Test;

public class TableSet : BaseTests
{
    [Test]
    public async Task Select_ToEntity()
    {
        var user = MysqlContext.Data.User.Where(o => o.Id == 1).Select(o => o.Id).ToEntity();
        Assert.True(user.Id == 1);
        Assert.IsNull(user.Name);

        var user2 = await MysqlContext.Data.User.Where(o => o.Id == 1).Select(o => o.Id).ToEntityAsync();
        Assert.AreEqual(user2.Id, 1);
        Assert.IsNull(user2.Name);
    }

    [Test]
    public void Select_ToList()
    {
        var lstAsc  = MysqlContext.Data.User.Asc(o => o.Id).ToList();
        var lstDesc = MysqlContext.Data.User.Desc(o => o.Id).ToList();
        var count   = MysqlContext.Data.User.Count();

        Assert.NotZero(lstAsc.Count);
        Assert.AreEqual(lstAsc.Count, count);
        Assert.NotNull(lstAsc[0].Name);

        Assert.AreEqual(lstAsc.FirstOrDefault().Id.GetValueOrDefault(), lstDesc.LastOrDefault().Id.GetValueOrDefault());
    }

    [Test]
    public void ToTopList()
    {
        var lst = MysqlContext.Data.User.Asc(o => o.Id).ToList(5);
        Assert.AreEqual(lst.Count, 5);
    }

    [Test]
    public void ToSplitList()
    {
        var lst  = MysqlContext.Data.User.Asc(o => o.Id).ToList(10, 2);
        var lst2 = MysqlContext.Data.User.Asc(o => o.Id).ToList(20);
        Assert.AreEqual(lst.Count, 10);
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(lst[i].Id.GetValueOrDefault(), lst2[i + 10].Id.GetValueOrDefault());
        }
    }

    [Test]
    public void Add_Update_Delete()
    {
        // 添加
        MysqlContext.Data.User.Insert(new UserPO()
        {
            Name = FarseerApplication.AppId.ToString()
        });

        var userPO = MysqlContext.Data.User.Where(o => o.Name == FarseerApplication.AppId.ToString()).ToEntity();
        Assert.AreEqual(userPO.Name, FarseerApplication.AppId.ToString());

        // 修改
        MysqlContext.Data.User.Where(o => o.Id == userPO.Id).Update(new UserPO()
        {
            Age = 1986
        });
        
        userPO = MysqlContext.Data.User.Where(o => o.Name == FarseerApplication.AppId.ToString()).ToEntity();
        Assert.AreEqual(userPO.Age.GetValueOrDefault(), 1986);
        Assert.AreEqual(userPO.Name, FarseerApplication.AppId.ToString());

        // 删除
        MysqlContext.Data.User.Where(o => o.Id == userPO.Id).Delete();

        var count = MysqlContext.Data.User.Where(o => o.Name == FarseerApplication.AppId.ToString()).Count();
        Assert.Zero(count);

    }

    [Test]
    public async Task Count_Without_Condition()
    {
        var count  = MysqlContext.Data.User.Count();
        var count2 = await MysqlContext.Data.User.CountAsync();
        Console.WriteLine($"数量：{count}");
        Assert.NotZero(count);
        Assert.AreEqual(count, count2);
    }

    [Test]
    public async Task Count_Condition()
    {
        using var lst    = MysqlContext.Data.User.ToList();
        var       count  = lst.Count(o => o.Age                    > 50 && o.Name.StartsWith("farseer-1"));
        var       count2 = MysqlContext.Data.User.Where(o => o.Age > 50 && o.Name.StartsWith("farseer-1")).Count();
        var       count3 = await MysqlContext.Data.User.Where(o => o.Age > 50 && o.Name.StartsWith("farseer-1")).CountAsync();
        Console.WriteLine($"数量：{count}");
        Assert.AreEqual(count, count2);
        Assert.AreEqual(count2, count3);
    }

    [Test]
    public async Task Count_Condition2()
    {
        var dataUser = MysqlContext.Data.User;
        var count1   = dataUser.Where(o => o.Age > 18).Count();
        var count2   = await dataUser.Where(o => o.Age < 18).CountAsync();
        Assert.NotZero(count1);
        Assert.NotZero(count2);
    }
}