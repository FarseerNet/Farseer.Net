using System;
using System.Linq;
using System.Threading.Tasks;
using Farseer.Net.Data.Test.Repository;
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
        Assert.True(user2.Id == 1);
        Assert.IsNull(user2.Name);
    }

    [Test]
    public void Select_ToList()
    {
        var list  = MysqlContext.Data.User.ToList();
        var count = MysqlContext.Data.User.Count();

        Assert.True(list.Count == count);
        Assert.NotZero(list.Count);
        Assert.NotNull(list[0].Name);
        
        
    }

    [Test]
    public async Task Count_Without_Condition()
    {
        var count  = MysqlContext.Data.User.Count();
        var count2 = await MysqlContext.Data.User.CountAsync();
        Console.WriteLine($"数量：{count}");
        Assert.NotZero(count);
        Assert.True(count == count2);
    }

    [Test]
    public async Task Count_Condition()
    {
        using var lst    = MysqlContext.Data.User.ToList();
        var       count  = lst.Count(o => o.Age                    > 50 && o.Name.StartsWith("farseer-1"));
        var       count2 = MysqlContext.Data.User.Where(o => o.Age > 50 && o.Name.StartsWith("farseer-1")).Count();
        var       count3 = await MysqlContext.Data.User.Where(o => o.Age > 50 && o.Name.StartsWith("farseer-1")).CountAsync();
        Console.WriteLine($"数量：{count}");
        Assert.True(count  == count2);
        Assert.True(count2 == count3);
    }

    [Test]
    public async Task Count_Condition2()
    {
        var dataUser = MysqlContext.Data.User;
        var count1   = dataUser.Where(o => o.Age > 18).Count();
        var count2   = await dataUser.Where(o => o.Age < 18).CountAsync();
        Assert.True(count1 > 0);
        Assert.True(count2 > 0);
    }
}