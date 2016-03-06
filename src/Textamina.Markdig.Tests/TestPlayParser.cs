// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
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
            var text = @"
+-----------------------------------+--------------------------------------+
| - this is a list                  | > We have a blockquote
| - this is a second item           |
|                                   |
| ```                               |
| Yes                               |
| ```                               |
+===================================+======================================+
| This is a second line             | 
+-----------------------------------+--------------------------------------+

/| we have mult | paragraph    |
/| we have a new colspan with a long line
/| and lots of text
";

            //            var reader = new StringReader(@"> > toto tata
            //> titi toto
            //");

            //var result = Markdown.ToHtml(text, new MarkdownPipeline().UseFootnotes().UseStrikethroughSuperAndSubScript());
            var result = Markdown.ToHtml(text, new MarkdownPipeline().UseAttributes().UseGridTable());
            //File.WriteAllText("test.html", result, Encoding.UTF8);
            Console.WriteLine(result);
        }
   }
}