using System.Linq;
using Farseer.Net.Cache.Test.Repository;
using NUnit.Framework;

namespace Farseer.Net.Cache.Test;

public class AopCacheTest : BaseTests
{
    [Test]
    public void LocalCache()
    {
        var userService = new UserService();

        // 第一次：读取数据，让Cache缓存数据
        var lst1 = userService.ToList();
        // 因为缓存还没有数据，此处应该=1
        Assert.AreEqual(userService.OprCount, 1);
        // 模拟数据100条
        Assert.AreEqual(lst1.Count, 100);


        // 第二次：判断是否还有数据库操作。
        var lst2 = userService.ToList();
        // 已经缓存数据了，不应该再操作数据库了，此处应该还是=1
        Assert.AreEqual(userService.OprCount, 1);
        // 模拟数据100条
        Assert.AreEqual(lst2.Count, 100);


        // 测试：添加数据
        var newUser = new UserPO { Id = 101, Name = "test-add", Age = 1001 };
        userService.Add(newUser);

        // 判断缓存与数据库，是否则时都增加了一条数据
        var lst3           = userService.ToList();
        var lst3ByDatabase = new DbContext().ToList();
        // 已经缓存数据了，不应该再操作数据库了，此处应该还是=1
        Assert.AreEqual(userService.OprCount, 1);
        // 缓存与数据库的数量应该一样
        Assert.AreEqual(lst3.Count, lst3ByDatabase.Count);
        Assert.AreEqual(lst3.Count, newUser.Id);
        Assert.IsTrue(lst3.Any(o => o.Id           == newUser.Id));
        Assert.IsTrue(lst3ByDatabase.Any(o => o.Id == newUser.Id));

        // 测试：修改数据
        userService.Update(99, new UserPO() { Name = "test-update", Age = 1002 });
        var lst4           = userService.ToList();
        var lst4ByDatabase = new DbContext().ToList();
        // 已经缓存数据了，不应该再操作数据库了，此处应该还是=1
        Assert.AreEqual(userService.OprCount, 1);
        // 缓存与数据库的数量应该一样
        Assert.AreEqual(lst4.Count, lst4ByDatabase.Count);
        Assert.AreEqual(lst4.Find(o => o.Id           == 99).Name, "test-update");
        Assert.AreEqual(lst4ByDatabase.Find(o => o.Id == 99).Name, "test-update");

        // 测试：删除数据
        userService.Delete(88);
        var lst5           = userService.ToList();
        var lst5ByDatabase = new DbContext().ToList();
        Assert.IsTrue(lst5.All(o => o.Id           != 88));
        Assert.IsTrue(lst5ByDatabase.All(o => o.Id != 88));
        Assert.AreEqual(lst5.Count, 100);
        Assert.AreEqual(lst5ByDatabase.Count, 100);
    }
}