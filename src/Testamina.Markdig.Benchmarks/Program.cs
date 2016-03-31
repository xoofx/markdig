// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
extern alias newcmark;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics;
using BenchmarkDotNet.Running;
using Textamina.Markdig;


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

        [Benchmark]
        public void TestMarkdig()
        {
            //var reader = new StreamReader(File.Open("spec.md", FileMode.Open));
            Markdown.ToHtml(text);
            //File.WriteAllText("spec.html", writer.ToString());
        }

        //[Benchmark]
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
            global::CommonMark.CommonMarkConverter.Convert(text);
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

        //[Benchmark]
        public void TestMarkdownDeep()
        {
            new MarkdownDeep.Markdown().Transform(text);
        }

        //[Benchmark]
        public void TestMarkdownSharp()
        {
            new MarkdownSharp.Markdown().Transform(text);
        }

        //[Benchmark]
        //public void TestMoonshine()
        //{
        //    Sundown.MoonShine.Markdownify(text);
        //}

        static void Main(string[] args)
        {
            bool markdig = args.Length == 0;
            bool simpleBench = false;

            if (simpleBench)
            {
                var clock = Stopwatch.StartNew();
                var program = new Program();
                for (int i = 0; i < 1000; i++)
                {
                    if (markdig)
                    {
                        program.TestMarkdig();
                    }
                    else
                    {
                        program.TestCommonMarkNet();
                    }
                }
                Console.WriteLine((markdig ? "MarkDig" : "CommonMark") +  $" => time: {clock.ElapsedMilliseconds}ms");
                DumpGC();
            }
            else
            {
                //new TestMatchPerf().TestMatch();

                var gcDiagnoser = new GCDiagnoser();
                var config = DefaultConfig.Instance.With(gcDiagnoser);
                BenchmarkRunner.Run<Program>(config);
                //BenchmarkRunner.Run<TestMatchPerf>();
                //BenchmarkRunner.Run<TestStringPerf>();
            }
        }

        static void DumpGC()
        {
            Console.WriteLine($"gc0: {GC.CollectionCount(0)}");
            Console.WriteLine($"gc1: {GC.CollectionCount(1)}");
            Console.WriteLine($"gc2: {GC.CollectionCount(2)}");
        }
    }
}
