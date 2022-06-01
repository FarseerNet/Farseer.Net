using Collections.Pooled;

namespace Study;

public class MemoryAllocation
{
    private readonly List<string> _lst = new();
    public MemoryAllocation()
    {
        for (int i = 0; i < 100000; i++)
        {
            _lst.Add(Guid.NewGuid().ToString("N"));
        }
    }

    public void Show()
    {
        Console.WriteLine($"准备测试类型转换的堆分配大小");
        TestAllocatedBytes("IEnumerable<string> newList = lst", () =>
        {
            IEnumerable<string> newList = _lst;
        }, 3);

        TestAllocatedBytes("lst.AsEnumerable()", () =>
        {
            _lst.AsEnumerable();
        }, 3);

        TestAllocatedBytes("lst.ToArray()", () =>
        {
            _lst.ToArray();
        }, 3);

        TestAllocatedBytes("lst.ToList()", () =>
        {
            _lst.ToList();
        }, 3);

        TestAllocatedBytes("lst.AsEnumerable().ToList()", () =>
        {
            _lst.AsEnumerable().ToList();
        }, 3);

        TestAllocatedBytes("foreach (var s in lst.Select(o => o.Length))", () =>
        {
            foreach (var s in _lst.Select(o => o.Length)) { }
        }, 3);

        TestAllocatedBytes("lst.Select(o => o.Length).Count()", () =>
        {
            _lst.Select(o => o.Length).Count();
        }, 3);

        TestAllocatedBytes("lst.Select(o => o.Length).ToList()", () =>
        {
            var lst2 = _lst.Select(o => o.Length).ToList();
        }, 3);

        TestAllocatedBytes("lst.Select(o => o.Length).ToPooledList()", () =>
        {
            using var lst2 = _lst.Select(o => o.Length).ToPooledList();
        }, 3);

        TestAllocatedBytes("PooledList.Add()", () =>
        {
            using PooledList<string> lst = new();
            lst.Add("123123");
        }, 3);

        TestAllocatedBytes("List.Add()", () =>
        {
            List<string> lst = new();
            lst.Add("123123");
        }, 3);
    }

    private void TestAllocatedBytes(string testName, Action act, int count)
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