using System;
using System.Linq;
using System.Threading.Tasks;
using Farseer.Net.ElasticSearch.Test.Repository;
using FS.Extends;
using FS.Utils.Common;
using NUnit.Framework;

namespace Farseer.Net.ElasticSearch.Test;

public class IndexSet : BaseTests
{
    [Test]
    public void Client_NotNull()
    {
        Assert.NotNull(ElasticSearchContext.Data.User.Client);
    }

    [Test]
    public void Add_Update_Delete()
    {
        var id = Guid.NewGuid().ToString("N");
        // 新增
        var result = ElasticSearchContext.Data.User.Insert(new UserPo
        {
            Id       = id,
            UserName = $"farseer-{Rand.GetRandom(10000, 99999)}",
            Age      = 0,
            Desc     = "hi,Im tester",
            CreateAt = DateTime.Now.ToTimestamps()
        });

        Assert.IsTrue(result);
        ElasticSearchContext.Data.User.RefreshIndex();

        var user = ElasticSearchContext.Data.User.Where(o => o.Id == id).ToEntity();
        Assert.NotNull(user);
        Assert.AreEqual(user.Id, id);
        Assert.AreEqual(user.Age, 0);

        // 修改用户
        result = ElasticSearchContext.Data.User.Where(o => o.Id == id).Update(new UserPo()
        {
            Age = 88
        });

        Assert.IsTrue(result);
        ElasticSearchContext.Data.User.RefreshIndex();

        user = ElasticSearchContext.Data.User.Where(o => o.Id == id).ToEntity();
        Assert.NotNull(user);
        Assert.AreEqual(user.Id, id);
        Assert.AreEqual(user.Age, 88);

        // 删除
        result = ElasticSearchContext.Data.User.Where(o => o.Id == id).Delete();
        Assert.NotNull(user);
        var count = ElasticSearchContext.Data.User.Where(o => o.Id == id).Count();
        Assert.Zero(count);
    }

    [Test]
    public async Task Count_Without_Condition()
    {
        var count  = ElasticSearchContext.Data.User.Count();
        var count2 = await ElasticSearchContext.Data.User.CountAsync();
        Console.WriteLine($"数量：{count}");
        Assert.AreEqual(count, count2);
    }

    [Test]
    public async Task Count_Condition()
    {
        using var lst    = ElasticSearchContext.Data.User.ToList();
        var       count  = lst.Count(o => o.Age                            > 50 && o.UserName.StartsWith("farseer-1"));
        var       count2 = ElasticSearchContext.Data.User.Where(o => o.Age > 50 && o.UserName.StartsWith("farseer-1")).Count();
        var       count3 = await ElasticSearchContext.Data.User.Where(o => o.Age > 50 && o.UserName.StartsWith("farseer-1")).CountAsync();
        Console.WriteLine($"数量：{count}");
        Assert.AreEqual(count, count2);
        Assert.AreEqual(count2, count3);
    }

    [Test]
    public async Task Count_Condition2()
    {
        var dataUser = ElasticSearchContext.Data.User;
        var count1   = dataUser.Where(o => o.Age > 18).Count();
        var count2   = await dataUser.Where(o => o.Age < 18).CountAsync();
        Assert.AreEqual(count1, count2);
    }
}