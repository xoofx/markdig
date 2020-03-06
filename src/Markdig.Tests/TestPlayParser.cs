// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.
using System;
using System.Linq;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestPlayParser
    {
        [Test]
        public void TestLink()
        {
            var doc = Markdown.Parse("There is a ![link](/yoyo)");
            var link = doc.Descendants<ParagraphBlock>().SelectMany(x => x.Inline.Descendants<LinkInline>()).FirstOrDefault(l => l.IsImage);
            Assert.AreEqual("/yoyo", link?.Url);
        }

        [Test]
        public void TestListBug2()
        {
            TestParser.TestSpec("10.\t*test* – test\n\n11.\t__test__ test\n\n", @"<ol start=""10"">
<li><p><em>test</em> – test</p>
</li>
<li><p><strong>test</strong> test</p>
</li>
</ol>
");
        }

        [Test]
        public void TestSimple()
        {
            var text = @" *[HTML]: Hypertext Markup Language

Later in a text we are using HTML and it becomes an abbr tag HTML
";

            //            var reader = new StringReader(@"> > toto tata
            //> titi toto
            //");

            //var result = Markdown.ToHtml(text, new MarkdownPipeline().UseFootnotes().UseEmphasisExtras());
            var result = Markdown.ToHtml(text, new MarkdownPipelineBuilder().UseAbbreviations().Build());
            //File.WriteAllText("test.html", result, Encoding.UTF8);
            Console.WriteLine(result);
        }

        [Test]
        public void TestEmptyLiteral()
        {
            var text = @"> *some text*
> some other text";
            var doc = Markdown.Parse(text);

            Assert.True(doc.Descendants<LiteralInline>().All(x => !x.Content.IsEmpty),
                "There should not have any empty literals");
        }

        [Test]
        public void TestSelfPipeline1()
        {
            var text = @" <!--markdig:pipetables-->

a | b
- | -
0 | 1
";
            TestParser.TestSpec(text, @"<!--markdig:pipetables-->
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
</tbody>
</table>
", "self");
        }

        [Test]
        public void TestListBug()
        {
            // TODO: Add this test back to the CommonMark specs
            var text = @"- item1
  - item2
    - item3
      - item4";
            TestParser.TestSpec(text, @"<ul>
<li>item1
<ul>
<li>item2
<ul>
<li>item3
<ul>
<li>item4</li>
</ul></li>
</ul></li>
</ul></li>
</ul>
");
        }


        [Test]
        public void TestHtmlBug()
        {
            TestParser.TestSpec(@" # header1

<pre class='copy'>
blabla
</pre>

# header2
", @"<h1>header1</h1>
<pre class='copy'>
blabla
</pre>
<h1>header2</h1>");
        }

        [Test]
        public void TestHtmlh4Bug()
        {
            TestParser.TestSpec(@"<h4>foobar</h4>", @"<h4>foobar</h4>");
        }

        [Test]
        public void TestStandardUriEscape()
        {
            TestParser.TestSpec(@"![你好](你好.png)", "<p><img src=\"你好.png\" alt=\"你好\" /></p>", "nonascii-noescape");
        }


        [Test]
        public void TestBugAdvanced()
        {
            TestParser.TestSpec(@"`https://{domain}/callbacks`
#### HEADING
Paragraph
", "<p><code>https://{domain}/callbacks</code></p>\n<h4 id=\"heading\">HEADING</h4>\n<p>Paragraph</p>", "advanced");
        }


        [Test]
        public void TestBugEmphAttribute()
        {
            // https://github.com/lunet-io/markdig/issues/108
            TestParser.TestSpec(@"*test*{name=value}", "<p><em name=\"value\">test</em></p>", "advanced");
        }

        [Test]
        public void TestBugPipeTables()
        {
            // https://github.com/lunet-io/markdig/issues/73
            TestParser.TestSpec(@"| abc | def |
| --- | --- |
| 1 | ~3 |
", @"<table>
<thead>
<tr>
<th>abc</th>
<th>def</th>
</tr>
</thead>
<tbody>
<tr>
<td>1</td>
<td>~3</td>
</tr>
</tbody>
</table>", "advanced");
        }

        [Test]
        public void TestGridTableWithCustomAttributes() {

            var input = @"
{.table}
+---+---+
| a | b |
+===+===+
| 1 | 2 |
+---+---+
";

            var expected = @"<table class=""table"">
<col style=""width:50%"" />
<col style=""width:50%"" />
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>1</td>
<td>2</td>
</tr>
</tbody>
</table>
";
            TestParser.TestSpec(input, expected, "advanced");
        }

        [Test]
        public void TestSamePipelineAllExtensions()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            // Reuse the same pipeline
            var result1 = Markdown.ToHtml("This is a \"\"citation\"\"", pipeline);
            var result2 = Markdown.ToHtml("This is a \"\"citation\"\"", pipeline);

            Assert.AreEqual("<p>This is a <cite>citation</cite></p>", result1.Trim());
            Assert.AreEqual(result1, result2);
        }

        // Test for emoji and smileys
        //        var text = @" This is a test with a :) and a :angry: smiley";


        // Test for definition lists:
        //
        //        var text = @"
        //Term 1
        //:   This is a definition item
        //    With a paragraph
        //    > This is a block quote

        //    - This is a list
        //    - item2

        //    ```java
        //    Test


        //    ```

        //    And a lazy line
        //:   This ia another definition item

        //Term2
        //Term3 *with some inline*
        //:   This is another definition for term2
        //";


        // Test for grid table


        //        var text = @"
        //+-----------------------------------+--------------------------------------+
        //| - this is a list                  | > We have a blockquote
        //| - this is a second item           |
        //|                                   |
        //| ```                               |
        //| Yes                               |
        //| ```                               |
        //+===================================+======================================+
        //| This is a second line             |
        //+-----------------------------------+--------------------------------------+

        //:::spoiler  {#yessss}
        //This is a spoiler
        //:::

        ///| we have mult | paragraph    |
        ///| we have a new colspan with a long line
        ///| and lots of text
        //";


    }
}