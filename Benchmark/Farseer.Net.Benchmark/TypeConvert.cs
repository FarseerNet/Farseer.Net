using BenchmarkDotNet.Attributes;
using FS.Data.Client.SqLite;
using FS.Extends;

namespace Farseer.Net.Benchmark;

public class TypeConvert : AbsBenchmark
{
    //[Benchmark]
    public void GetFilePath1()
    {
        //int.Parse("3");
        //new SqLiteConnectionString().GetFilePath1("/aaaf/bccc\\deeef//aaaa");
    }
    
    [Benchmark]
    public void GetFilePath2()
    {
        //"3".ConvertType(0);
        new SqLiteConnectionString().GetFilePath("/aaaf/bccc\\deeef//aaaa");
    }
}