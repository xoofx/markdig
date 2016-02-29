using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Sundown;
using Textamina.Markdig;
using Textamina.Markdig.Parsers;

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
            Markdown.ConvertToHtml(text);
            //File.WriteAllText("spec.html", writer.ToString());
        }

        [Benchmark]
        public void TestCommonMark()
        {
            ////var reader = new StreamReader(File.Open("spec.md", FileMode.Open));
            var reader = new StringReader(text);
            //CommonMark.CommonMarkConverter.Parse(reader);
            //CommonMark.CommonMarkConverter.Parse(reader);
            //reader.Dispose();
            CommonMark.CommonMarkConverter.Convert(reader, new StringWriter());
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
            MoonShine.Markdownify(text);
        }

        static void Main(string[] args)
        {
            bool markdig = args.Length == 0;
            bool simpleBench = false;

            if (simpleBench)
            {
                var clock = Stopwatch.StartNew();
                var program = new Program();
                for (int i = 0; i < 500; i++)
                {
                    if (markdig)
                    {
                        program.TestMarkdig();
                    }
                    else
                    {
                        program.TestCommonMark();
                    }
                }
                Console.WriteLine((markdig ? "MarkDig" : "CommonMark") +  $" => time: {clock.ElapsedMilliseconds}ms");
                DumpGC();
            }
            else
            {
                BenchmarkRunner.Run<Program>();
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
