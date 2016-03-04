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
{#heading}
|a  | b
|---|---
| 0 | 1

# Heading {#heading}

```info {#test .class src=tata src2='toto'}
This is a test
```
";

            //            var reader = new StringReader(@"> > toto tata
            //> titi toto
            //");

            //var result = Markdown.ConvertToHtml(text, new MarkdownPipeline().UseFootnotes().UseStrikethroughSuperAndSubScript());
            var result = Markdown.ConvertToHtml(text, new MarkdownPipeline().UseAttributes().UsePipeTable());
            //File.WriteAllText("test.html", result, Encoding.UTF8);
            Console.WriteLine(result);
        }
   }
}