using Collections.Pooled;
using Farseer.Net.Benchmark.Data;

namespace Farseer.Net.Benchmark.Other;

public class TestUsingPooled
{
    public List<UserPO> lst = new();
    public UserPO       User;

    public TestUsingPooled()
    {
        for (int i = 0; i < 100000; i++)
        {
            lst.Add(new UserPO() { Name = "123123" });
        }
    }
    public void Test()
    {
        TestToList();
        TestToPooledList();
        TestToList_ToPooledList();
    }
    
    public void TestToList()
    {
        GC.Collect();
        var baseAllocated = GC.GetTotalAllocatedBytes();
        Console.WriteLine($"准备测试ToList的堆分配大小");
        for (int j = 1; j <= 5; j++)
        {
            for (int i = 0; i < 1000; i++)
            {
                var lst2 = lst.Select(o => o.Name).ToList();
            }
            Console.WriteLine($"{j}/5:ToList：当前已分配堆大小：{(GC.GetTotalAllocatedBytes() - baseAllocated).ToString("N0")} byte");
        }
        Console.WriteLine("---------------------------");
    }

    public void TestToPooledList()
    {
        GC.Collect();
        var baseAllocated = GC.GetTotalAllocatedBytes();
        Console.WriteLine($"准备测试ToPooledList的堆分配大小");
        for (int j = 1; j <= 5; j++)
        {
            for (int i = 0; i < 1000; i++)
            {
                using var lst2 = lst.Select(o => o.Name).ToPooledList();
            }
            Console.WriteLine($"{j}/5:ToPooledList：当前已分配堆大小：{(GC.GetTotalAllocatedBytes() - baseAllocated).ToString("N0")} byte");
        }
    }

    public void TestToList_ToPooledList()
    {
        GC.Collect();
        var baseAllocated = GC.GetTotalAllocatedBytes();
        Console.WriteLine($"准备测试ToList().ToPooledList的堆分配大小");
        for (int j = 1; j <= 5; j++)
        {
            for (int i = 0; i < 1000; i++)
            {
                using var lst2 = lst.Select(o => o.Name).ToList().ToPooledList();
            }
            Console.WriteLine($"{j}/5:ToList().ToPooledList：当前已分配堆大小：{(GC.GetTotalAllocatedBytes() - baseAllocated).ToString("N0")} byte");
        }
    }
}