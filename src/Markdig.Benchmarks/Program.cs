// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
extern alias newcmark;
using System;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Markdig;


namespace Testamina.Markdig.Benchmarks
{
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
        [Benchmark]
        public void TestMarkdig()
        {
            //var reader = new StreamReader(File.Open("spec.md", FileMode.Open));
            Markdown.ToHtml(text);
            //File.WriteAllText("spec.html", writer.ToString());
        }

        [Benchmark]
        public void TestCommonMarkCpp()
        {
            //var reader = new StreamReader(File.Open("spec.md", FileMode.Open));
            CommonMarkLib.ToHtml(text);
            //File.WriteAllText("spec.html", writer.ToString());
        }

        [Benchmark]
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

        [Benchmark]
        public void TestCommonMarkNetNew()
        {
            ////var reader = new StreamReader(File.Open("spec.md", FileMode.Open));
            // var reader = new StringReader(text);
            //CommonMark.CommonMarkConverter.Parse(reader);
            //CommonMark.CommonMarkConverter.Parse(reader);
            //reader.Dispose();
            //var writer = new StringWriter();
            newcmark::CommonMark.CommonMarkConverter.Convert(text);
            //writer.Flush();
            //writer.ToString();
        }

        [Benchmark]
        public void TestMarkdownDeep()
        {
            new MarkdownDeep.Markdown().Transform(text);
        }

        [Benchmark]
        public void TestMarkdownSharp()
        {
            new MarkdownSharp.Markdown().Transform(text);
        }

        [Benchmark]
        public void TestMoonshine()
        {
            Sundown.MoonShine.Markdownify(text);
        }

        static void Main(string[] args)
        {
            bool markdig = args.Length == 0;
            bool simpleBench = false;

            if (simpleBench)
            {
                var clock = Stopwatch.StartNew();
                var program = new Program();

                GC.Collect(2, GCCollectionMode.Forced, true);

                var gc0 = GC.CollectionCount(0);
                var gc1 = GC.CollectionCount(1);
                var gc2 = GC.CollectionCount(2);

                const int count = 12*64;
                for (int i = 0; i < count; i++)
                {
                    if (markdig)
                    {
                        program.TestMarkdig();
                    }
                    else
                    {
                        program.TestCommonMarkNetNew();
                    }
                }
                clock.Stop();
                Console.WriteLine((markdig ? "MarkDig" : "CommonMark") +  $" => time: {(double)clock.ElapsedMilliseconds/count}ms (total {clock.ElapsedMilliseconds}ms)");
                DumpGC(gc0, gc1, gc2);
            }
            else
            {
                //new TestMatchPerf().TestMatch();

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

        private static void DumpGC(int gc0, int gc1, int gc2)
        {
            Console.WriteLine($"gc0: {GC.CollectionCount(0)-gc0}");
            Console.WriteLine($"gc1: {GC.CollectionCount(1)-gc1}");
            Console.WriteLine($"gc2: {GC.CollectionCount(2)-gc2}");
        }
    }
}
