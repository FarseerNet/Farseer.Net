// See https://aka.ms/new-console-template for more information

//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(Program).Assembly);

using Farseer.Net.Benchmark;
using Farseer.Net.Benchmark.Farseer.Net;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

var testUsingPooled = new TestUsingPooled();
var lst             = testUsingPooled.Test();
// Console.WriteLine(lst.Count);
// Console.WriteLine(testUsingPooled.User.Name);
// lst.Dispose();
// Console.WriteLine(testUsingPooled.User.Name);
// Console.WriteLine(lst.Count);
//BenchmarkDotNet.Running.BenchmarkRunner.Run<Span_Interpolation_Extend>();