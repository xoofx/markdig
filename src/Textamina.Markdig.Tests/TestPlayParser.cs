using System;
using System.IO;
using System.Text;
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
            var text = @"This is a link to a footnote[^OhYeah] [^OhYeah]

[^OhYeah]: This is the footnote
    > Yes

qqewtqet

qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet


qqewtqet



This is a text after the footnote not part of the foot note [^3]

[^3]: This is a 2nd footnote
";



//            var reader = new StringReader(@"> > toto tata
//> titi toto
//");

            var result = Markdown.ConvertToHtml(text, new MarkdownPipeline().EnableFootnoteExtensions());
            File.WriteAllText("test.html", result, Encoding.UTF8);
            Console.WriteLine(result);
        }
   }
}