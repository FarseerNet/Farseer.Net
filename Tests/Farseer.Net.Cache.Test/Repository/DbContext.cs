using System;
using System.Collections.Generic;
using System.Linq;
using FS.Utils.Common;

namespace Farseer.Net.Cache.Test.Repository;

/// <summary>
/// 模拟数据库操作
/// </summary>
public class DbContext
{
    public static readonly Dictionary<int, UserPO> Cache = new();

    // 初始化数据
    static DbContext()
    {
        for (int i = 0; i < 100; i++)
        {
            Cache[i] = new UserPO { Id = i, Name = Guid.NewGuid().ToString("N"), Age = Rand.GetRandom(1, 100) };
        }
    }

    /// <summary>
    /// 获取数据库集合
    /// </summary>
    /// <returns></returns>
    public List<UserPO> ToList() => Cache.Select(o => o.Value).ToList();

    /// <summary>
    /// 模拟数据库添加操作
    /// </summary>
    public void Add(UserPO user) => Cache.TryAdd(user.Id, user);

    /// <summary>
    /// 模拟数据库更新数据
    /// </summary>
    public void Update(int id, UserPO user)
    {
        if (Cache.ContainsKey(id)) Cache[id] = user;
    }

    /// <summary>
    /// 模拟数据库删除
    /// </summary>
    public void Delete(int id) => Cache.Remove(id);
}