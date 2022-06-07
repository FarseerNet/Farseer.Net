using System.Collections.Generic;
using FS.Cache.Attribute;

namespace Farseer.Net.Cache.Test.Repository;

public class UserService
{
    /// <summary>
    /// 代表实际查询数据库的次数
    /// </summary>
    public int OprCount = 0;

    /// <summary>
    /// 获取数据库集合
    /// </summary>
    [Cache("user")]
    public List<UserPO> ToList()
    {
        OprCount++;
        return new DbContext().ToList();
    }

    /// <summary>
    /// 模拟数据库添加操作
    /// </summary>
    [Cache("user")]
    public void Add(UserPO user) => new DbContext().Add(user);

    /// <summary>
    /// 模拟数据库更新数据
    /// </summary>
    [Cache("user")]
    public void Update(int id, UserPO user)
    {
        user.Id = id;
        new DbContext().Update(id, user);
    }

    /// <summary>
    /// 模拟数据库删除
    /// </summary>
    [Cache("user")]
    public void Delete(int id) => new DbContext().Delete(id);
}