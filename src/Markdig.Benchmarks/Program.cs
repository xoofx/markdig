// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

using Markdig;


namespace Testamina.Markdig.Benchmarks;

//[BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit, processCount: 1, warmupIterationCount: 2)]
public class Program
{
    private string text;

    public Program()
    {
        //text = File.ReadAllText("progit.md");
        text = File.ReadAllText("spec.md");
    }

    //[Benchmark(Description = "TestMarkdig", OperationsPerInvoke = 4096)]
    [Benchmark(Description = "markdig")]
    public void TestMarkdig()
    {
        //var reader = new StreamReader(File.Open("spec.md", FileMode.Open));
        Markdown.ToHtml(text);
        //File.WriteAllText("spec.html", writer.ToString());
    }

    [Benchmark(Description = "cmark")]
    public void TestCommonMarkCpp()
    {
        //var reader = new StreamReader(File.Open("spec.md", FileMode.Open));
        CommonMarkLib.ToHtml(text);
        //File.WriteAllText("spec.html", writer.ToString());
    }

    [Benchmark(Description = "CommonMark.NET")]
    public void TestCommonMarkNet()
    {
        ////var reader = new StreamReader(File.Open("spec.md", FileMode.Open));
        // var reader = new StringReader(text);
        //CommonMark.CommonMarkConverter.Parse(reader);
        //CommonMark.CommonMarkConverter.Parse(reader);
        //reader.Dispose();
        //var writer = new StringWriter();
        CommonMark.CommonMarkConverter.Convert(text);
        //writer.Flush();
        //writer.ToString();
    }

    [Benchmark(Description = "MarkdownSharp")]
    public void TestMarkdownSharp()
    {
        new MarkdownSharp.Markdown().Transform(text);
    }

    static void Main(string[] args)
    {
        var config = ManualConfig.Create(DefaultConfig.Instance);
        //var gcDiagnoser = new MemoryDiagnoser();
        //config.Add(new Job { Mode = Mode.SingleRun, LaunchCount = 2, WarmupCount = 2, IterationTime = 1024, TargetCount = 10 });
        //config.Add(new Job { Mode = Mode.Throughput, LaunchCount = 2, WarmupCount = 2, TargetCount = 10 });
        //config.Add(gcDiagnoser);

        //var  config = DefaultConfig.Instance;
        BenchmarkRunner.Run<Program>(config);
        //BenchmarkRunner.Run<TestDictionary>(config);
        //BenchmarkRunner.Run<TestMatchPerf>();
        //BenchmarkRunner.Run<TestStringPerf>();
    }
}
