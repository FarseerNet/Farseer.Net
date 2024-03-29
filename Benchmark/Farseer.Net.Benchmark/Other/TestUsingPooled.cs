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

        TestAllocatedBytes("lst.Select(o => o.Name)", () =>
        {
            var lst2 = lst.Select(o => o.Name);
        }, 3);

        TestAllocatedBytes("foreach (var s in lst.Select(o => o.Name))", () =>
        {
            foreach (var s in lst.Select(o => o.Name)) { }
        }, 3);

        TestAllocatedBytes("lst.Select(o => o.Name).Count()", () =>
        {
            lst.Select(o => o.Name).Count();
        }, 3);

        TestAllocatedBytes("lst.Select(o => o.Name).ToList()", () =>
        {
            var lst2 = lst.Select(o => o.Name).ToList();
        }, 3);

        TestAllocatedBytes("lst.Select(o => o.Name).ToPooledList()", () =>
        {
            using var lst2 = lst.Select(o => o.Name).ToPooledList();
        }, 3);

        TestAllocatedBytes("PooledList.Add()", () =>
        {
            using PooledList<UserPO> lst = new();
            lst.Add(new UserPO() { Name = "123123" });
        }, 3);

        TestAllocatedBytes("List.Add()", () =>
        {
            List<UserPO> lst = new();
            lst.Add(new UserPO() { Name = "123123" });
        }, 3);
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