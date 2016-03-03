using System;
using System.IO;
using NUnit.Framework;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Tests
{
    [TestFixture]
    public class TestPlayParser
    {
        [Test]
        public void TestSimple()
        {
            var text = @"[^1]: This is a footnote

This is a link to a footnote [^2] [^1]

[^2]: This is second a footnote
";



//            var reader = new StringReader(@"> > toto tata
//> titi toto
//");

            var result = Markdown.ConvertToHtml(text, new MarkdownPipeline().EnableFootnoteExtensions());
            Console.WriteLine(result);
        }
   }
}