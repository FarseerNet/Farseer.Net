using Collections.Pooled;
using Farseer.Net.Benchmark.Data;

namespace Farseer.Net.Benchmark.Other;

public class TestUsingPooled
{
    public PooledList<UserPO> lst = new();
    public UserPO             User;

    public TestUsingPooled()
    {
        for (int i = 0; i < 100000; i++)
        {
            lst.Add(new UserPO() { Name = "123123" });
        }
    }

    public void Test()
    {
        TestClassConvert();
        // TestToList();
        // TestToPooledList();
        // TestToList_ToPooledList();
    }

    public void TestClassConvert()
    {
        Console.WriteLine($"准备测试类型转换的堆分配大小");
        TestAllocatedBytes("IEnumerable<UserPO> newUser = lst", () =>
        {
            IEnumerable<UserPO> newUser = lst;
        }, 3);

        TestAllocatedBytes("lst.AsEnumerable()", () =>
        {
            lst.AsEnumerable();
        }, 3);

        TestAllocatedBytes("lst.ToArray()", () =>
        {
            lst.ToArray();
        }, 3);

        TestAllocatedBytes("lst.ToList()", () =>
        {
            lst.ToList();
        }, 3);

        TestAllocatedBytes("lst.AsEnumerable().ToList()", () =>
        {
            lst.AsEnumerable().ToList();
        }, 3);

        TestAllocatedBytes("PooledList.Add()", () =>
        {
            using PooledList<UserPO> lst = new(100000);
            for (int i = 0; i < 100000; i++)
            {
                lst.Add(new UserPO() { Name = "123123" });
            }
        }, 10);

        TestAllocatedBytes("List.Add()", () =>
        {
            List<UserPO> lst = new(100000);
            for (int i = 0; i < 100000; i++)
            {
                lst.Add(new UserPO() { Name = "123123" });
            }
        }, 10);
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

    public void TestAllocatedBytes(string testName, Action act, int count)
    {
        GC.Collect();
        for (int index = 1; index <= count; index++)
        {
            var callBefore = GC.GetAllocatedBytesForCurrentThread();
            act();
            var callAfter = GC.GetAllocatedBytesForCurrentThread();
            Console.WriteLine($"({index}/{count})\t{testName}：当前堆分配大小：{(callAfter - callBefore):N0} byte");
        }
        Console.WriteLine("------------------------------------------------------");
    }
}