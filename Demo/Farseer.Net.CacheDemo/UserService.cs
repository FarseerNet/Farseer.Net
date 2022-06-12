using Farseer.Net.CacheDemo.Repository;
using FS.AOP;
using FS.Cache.Attribute;
using FS.Core.AOP.LinkTrack;

namespace Farseer.Net.CacheDemo;

[Log]
public class UserService
{
    /// <summary> 获取数据集合 </summary>
    [Cache("user")]
    public IEnumerable<UserPO> ToList() => new DatabaseContext().ToList();
    /// <summary> 获取数据集合 </summary>
    [Cache("user")]
    public UserPO ToEntity()
    {
        //throw new ArgumentOutOfRangeException("bbbbbbb");
        return new DatabaseContext().ToList().FirstOrDefault();
    }

    /// <summary> 模拟数据库添加操作（缓存必须有一个唯一标识） </summary>
    [CacheUpdate("user")]
    public UserPO Add(UserPO user)
    {
        new DatabaseContext().Add(user);
        return user;
    }

    /// <summary> 模拟数据库更新数据 </summary>
    [CacheUpdate("user")]
    public UserPO Update(int id, UserPO user)
    {
        new DatabaseContext().Update(id, user);
        return user;
    }

    /// <summary> 模拟数据库删除 </summary>
    [CacheRemove("user")]
    public void Delete([CacheId] int id) => new DatabaseContext().Delete(id);
}