using Markdig.Helpers;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using NUnit.Framework;
using System.IO;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestExample
    {
        [Test]
        public void Test()
        {
            var markdown = $@"
# Test document
This document contains an unordered list. It uses tabs to indent. This test demonstrates
a method of making the input markdown uniform without altering any other markdown in the
resulting output file.

- item1

>look, ma:
> my space is not normalized!
";
            MarkdownDocument markdownDocument = Markdown.Parse(markdown, trackTrivia: true);
            var listBlock = markdownDocument[2] as ListBlock;
            var listItem = listBlock[0] as ListItemBlock;
            var paragraph = listItem[0] as ParagraphBlock;
            var containerInline = new ContainerInline();
            containerInline.AppendChild(new LiteralInline(" my own text!"));
            containerInline.AppendChild(new LineBreakInline { NewLine = NewLine.CarriageReturnLineFeed });
            paragraph.Inline = containerInline;

            var sw = new StringWriter();
            var rr = new RoundtripRenderer(sw);
            rr.Write(markdownDocument);
            var outputMarkdown = sw.ToString();
            var expected = $@"
# Test document
This document contains an unordered list. It uses tabs to indent. This test demonstrates
a method of making the input markdown uniform without altering any other markdown in the
resulting output file.

- my own text!

>look, ma:
> my space is not normalized!
";

            expected = expected.Replace("\r\n", "\n").Replace("\r", "\n");
            outputMarkdown = outputMarkdown.Replace("\r\n", "\n").Replace("\r", "\n");

            Assert.AreEqual(expected, outputMarkdown);
        }
    }
}
