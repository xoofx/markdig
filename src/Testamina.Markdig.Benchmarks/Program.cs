using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Textamina.Markdig.Formatters;
using Textamina.Markdig.Parsing;

namespace Testamina.Markdig.Benchmarks
{
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
            var reader = new MarkdownParser(new StringReader(text));
            var doc = reader.Parse();
            //var formatter = new HtmlFormatter(new StringWriter());
            //formatter.Write(doc);
        }

        [Benchmark]
        public void TestCommonMark()
        {
            //CommonMark.CommonMarkConverter.Convert(new StringReader(text), new StringWriter());
            CommonMark.CommonMarkConverter.Parse(new StringReader(text));
        }

        static void Main(string[] args)
        {
            var clock = Stopwatch.StartNew();
            var program = new Program();
            for (int i = 0; i < 200; i++)
            {
                program.TestMarkdig();
                //program.TestCommonMark();
            }
            Console.WriteLine($"time: {clock.ElapsedMilliseconds}ms");

            //BenchmarkRunner.Run<Program>();
        }
    }
}
