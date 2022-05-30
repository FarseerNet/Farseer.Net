// See https://aka.ms/new-console-template for more information

//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(Program).Assembly);

using Farseer.Net.Benchmark;
using FS.Data.Client.SqLite;
using FS.Extends;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

//"aaaaaa,bbbbbbb,ccccccc".AsSpan().Split(",");
//new SqLiteConnectionString().GetFilePath2("/aaaf/bccc\\deeef//aaaa");
BenchmarkDotNet.Running.BenchmarkRunner.Run<TypeConvert>();