// See https://aka.ms/new-console-template for more information

using Collections.Pooled;
using Study;

//new MemoryAllocation().Show();

var lst = new PooledList<string>();
lst.Add("123123");

var lst2 = lst.AsEnumerable();
lst.Dispose();
Console.WriteLine(lst.Count());
Console.WriteLine(lst2.Count());

//new EnumerableConvert().Show();