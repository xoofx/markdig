using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Textamina.Markdig.Formatters;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Tests
{
    [TestFixture]
    public class TestTableParser
    {
        [Test]
        public void TestSimple()
        {
            var reader = new StringReader(@"
| This is a test | With a column
-------------  | -------------
| 0              | 1

");
//            var reader = new StringReader(@"> > toto tata
//> titi toto
//");
            var parser = new MarkdownParser(reader);
            var document = parser.Parse();

            var output = new StringWriter();
            var formatter = new HtmlFormatter(output);
            formatter.Write(document);
            output.Flush();

            var result = output.ToString();
            Console.WriteLine(result);
        }
   }
}