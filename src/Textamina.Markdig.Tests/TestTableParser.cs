using System;
using System.IO;
using NUnit.Framework;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Tests
{
    [TestFixture]
    public class TestTableParser
    {
        [Test]
        public void TestSimple()
        {
            var text = @"
a | b 
0 | 1 | 2
3 | 4
5 |
";
//            var reader = new StringReader(@"> > toto tata
//> titi toto
//");

            var result = Markdown.ConvertToHtml(text, new MarkdownPipeline().EnablePipeTable());
            Console.WriteLine(result);
        }
   }
}