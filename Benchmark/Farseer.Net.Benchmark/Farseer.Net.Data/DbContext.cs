using BenchmarkDotNet.Attributes;
using FS.Data;

namespace Farseer.Net.Benchmark.Data;

public class DbContext : BaseBenchmark
{
    public static string Name = "abcdefghijk";
    // [Benchmark]
    // public void NewDbContext()
    // {
    //     using var db = new MysqlContext();
    // }
    //
    // [Benchmark]
    // public TableSet<UserPO> StaticDbContext()
    // {
    //     return MysqlContext.Data.User.Where(o => o.Age == 0);
    // }

    [Benchmark]
    public string StringSubstring()
    {
        return Name.Substring(3);
    }

    [Benchmark]
    public ReadOnlySpan<char> SpanSlice()
    {
        ReadOnlySpan<char> span   = Name;
        var                result = span.Slice(3);
        return result.ToString();
    }
}