// See https://aka.ms/new-console-template for more information

//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(Program).Assembly);

using Farseer.Net.Benchmark;
using FS.Data.Client.SqLite;
using FS.Extends;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;


var readOnlySpan1 = "aaa".AsMemory();
var readOnlySpan2 = "bbb".AsSpan();
var a=$"{readOnlySpan1}-{readOnlySpan2}";
Console.WriteLine(readOnlySpan1);

BenchmarkDotNet.Running.BenchmarkRunner.Run<TypeConvert>();