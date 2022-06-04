// See https://aka.ms/new-console-template for more information

//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(Program).Assembly);

using Farseer.Net.Benchmark;
using Farseer.Net.Benchmark.Farseer.Net;
using Farseer.Net.Benchmark.Other;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

//  var testUsingPooled = new TestUsingPooled();
// testUsingPooled.Test();
// Console.WriteLine(testUsingPooled.User.Name);
// lst.Dispose();
// Console.WriteLine(testUsingPooled.User.Name);
// Console.WriteLine(lst.Count);
BenchmarkDotNet.Running.BenchmarkRunner.Run<StackTraceSpeed>();