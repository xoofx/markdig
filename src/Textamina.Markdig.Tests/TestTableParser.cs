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
            var reader = new StringReader(@"
|a|b
|-|-|-
|0|1|2|3
|A|B

");
//            var reader = new StringReader(@"> > toto tata
//> titi toto
//");

            var result = Markdown.Convert(reader);
            Console.WriteLine(result);
        }
   }
}