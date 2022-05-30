// See https://aka.ms/new-console-template for more information

//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(Program).Assembly);

using Farseer.Net.Benchmark.Farseer.Net;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

Console.WriteLine(new Span_Interpolation_Extend().String_Interpolation());
Console.WriteLine(new Span_Interpolation_Extend().Span_Interpolation());
//BenchmarkDotNet.Running.BenchmarkRunner.Run<Span_Interpolation_Extend>();