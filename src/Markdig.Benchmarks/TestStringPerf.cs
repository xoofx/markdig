using System.IO;
using BenchmarkDotNet.Attributes;
using Markdig.Renderers;

namespace Testamina.Markdig.Benchmarks
{
    public class TestStringPerf
    {
        private string text;

        public TestStringPerf()
        {
            text = new string('a', 1000);
        }

        [Benchmark]
        public void TestIndexOfAny()
        {
            var writer = new HtmlRenderer(new StringWriter());
            for (int i = 0; i < 100; i++)
            {
                writer.WriteEscape(text, 0, text.Length);
                writer.WriteEscape(text, 0, text.Length);
                writer.WriteEscape(text, 0, text.Length);
                writer.WriteEscape(text, 0, text.Length);
                writer.WriteEscape(text, 0, text.Length);

                writer.WriteEscape(text, 0, text.Length);
                writer.WriteEscape(text, 0, text.Length);
                writer.WriteEscape(text, 0, text.Length);
                writer.WriteEscape(text, 0, text.Length);
                writer.WriteEscape(text, 0, text.Length);
            }
        }


        //[Benchmark]
        //public void TestCustomIndexOfAny()
        //{
        //    var writer = new HtmlRenderer(new StringWriter());
        //    for (int i = 0; i < 100; i++)
        //    {
        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //        writer.WriteEscapeOptimized(text, 0, text.Length);

        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //        writer.WriteEscapeOptimized(text, 0, text.Length);
        //    }
        //}

    }
}