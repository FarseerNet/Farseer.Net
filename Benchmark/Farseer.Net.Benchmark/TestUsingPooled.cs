using Collections.Pooled;
using Farseer.Net.Benchmark.Data;

namespace Farseer.Net.Benchmark;

public class TestUsingPooled
{
    public UserPO User;
    public PooledList<UserPO> Test()
    {
        var lst    = new PooledList<UserPO>();
        var userPO = new UserPO() { Name = "123123" };
        lst.Add(userPO);

        User = userPO;
        using var pooledList = lst.Select(o => o.Age).ToPooledList();
        lst.Dispose();
        Console.WriteLine(pooledList.Count);
        return lst;
    }
}