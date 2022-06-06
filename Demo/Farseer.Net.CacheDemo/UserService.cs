using Farseer.Net.CacheDemo.Repository;
using FS.Cache;
using FS.Cache.Attribute;

namespace Farseer.Net.CacheDemo;

//[CacheConfiguration("user",)]
public class UserService
{
    /// <summary>
    /// 获取数据库集合
    /// </summary>
    [Cache("user")]
    public IList<UserPO> ToList() => new DatabaseContext().ToList().ToList();

    /// <summary>
    /// 模拟数据库添加操作
    /// 缓存必须有一个唯一标识
    /// </summary>
    [CacheUpdate("user")]
    public UserPO Add(UserPO user)
    {
        new DatabaseContext().Add(user);
        return user;
    }

    /// <summary>
    /// 模拟数据库更新数据
    /// </summary>
    [CacheUpdate("user")]
    public UserPO Update(int id, UserPO user)
    {
        new DatabaseContext().Update(id, user);
        return user;
    }

    /// <summary>
    /// 模拟数据库删除
    /// </summary>
    [CacheRemove("user")]
    public void Delete([CacheId] int id) => new DatabaseContext().Delete(id);
}