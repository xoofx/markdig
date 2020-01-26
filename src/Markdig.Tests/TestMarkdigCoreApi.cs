using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using NUnit.Framework;
using System.IO;

namespace Markdig.Tests
{
    public class TestMarkdigCoreApi
    {
        [Test]
        public void TestToHtml()
        {
            string html = Markdown.ToHtml("This is a text with some *emphasis*");
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);

            html = Markdown.ToHtml("This is a text with a https://link.tld/");
            Assert.AreNotEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
        }

        [Test]
        public void TestToHtmlWithPipeline()
        {
            var pipeline = new MarkdownPipelineBuilder()
                .Build();

            string html = Markdown.ToHtml("This is a text with some *emphasis*", pipeline);
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);

            html = Markdown.ToHtml("This is a text with a https://link.tld/", pipeline);
            Assert.AreNotEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);

            pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            html = Markdown.ToHtml("This is a text with a https://link.tld/", pipeline);
            Assert.AreEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
        }

        [Test]
        public void TestToHtmlWithWriter()
        {
            StringWriter writer = new StringWriter();

            _ = Markdown.ToHtml("This is a text with some *emphasis*", writer);
            string html = writer.ToString();
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);

            writer = new StringWriter();
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            _ = Markdown.ToHtml("This is a text with a https://link.tld/", writer, pipeline);
            html = writer.ToString();
            Assert.AreEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);

        }

        [Test]
        public void TestConvert()
        {
            StringWriter writer = new StringWriter();
            HtmlRenderer renderer = new HtmlRenderer(writer);

            _ = Markdown.Convert("This is a text with some *emphasis*", renderer);
            string html = writer.ToString();
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);

            writer = new StringWriter();
            renderer = new HtmlRenderer(writer);
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            _ = Markdown.Convert("This is a text with a https://link.tld/", renderer, pipeline);
            html = writer.ToString();
            Assert.AreEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
        }

        [Test]
        public void TestParse()
        {
            const string markdown = "This is a text with some *emphasis*";

            var pipeline = new MarkdownPipelineBuilder()
                .UsePreciseSourceLocation()
                .Build();

            MarkdownDocument document = Markdown.Parse(markdown, pipeline);

            Assert.AreEqual(1, document.LineCount);
            Assert.AreEqual(markdown.Length, document.Span.Length);
            Assert.AreEqual(1, document.LineStartIndexes.Count);
            Assert.AreEqual(0, document.LineStartIndexes[0]);

            Assert.AreEqual(1, document.Count);
            ParagraphBlock paragraph = document[0] as ParagraphBlock;
            Assert.NotNull(paragraph);
            Assert.AreEqual(markdown.Length, paragraph.Span.Length);
            LiteralInline literal = paragraph.Inline.FirstChild as LiteralInline;
            Assert.NotNull(literal);
            Assert.AreEqual("This is a text with some ", literal.ToString());
            EmphasisInline emphasis = literal.NextSibling as EmphasisInline;
            Assert.NotNull(emphasis);
            Assert.AreEqual("*emphasis*".Length, emphasis.Span.Length);
            LiteralInline emphasisLiteral = emphasis.FirstChild as LiteralInline;
            Assert.NotNull(emphasisLiteral);
            Assert.AreEqual("emphasis", emphasisLiteral.ToString());
            Assert.Null(emphasisLiteral.NextSibling);
            Assert.Null(emphasis.NextSibling);
        }

        [Test]
        public void TestNormalize()
        {
            string normalized = Markdown.Normalize("Heading\n=======");
            Assert.AreEqual("# Heading", normalized);
        }

        public void TestNormalizeWithWriter()
        {
            StringWriter writer = new StringWriter();

            _ = Markdown.Normalize("Heading\n=======", writer);
            string normalized = writer.ToString();
            Assert.AreEqual("# Heading", normalized);
        }

        [Test]
        public void TestToPlainText()
        {
            string plainText = Markdown.ToPlainText("*Hello*, [world](http://example.com)!");
            Assert.AreEqual("Hello, world!\n", plainText);
        }

        [Test]
        public void TestToPlainTextWithWriter()
        {
            StringWriter writer = new StringWriter();

            _ = Markdown.ToPlainText("*Hello*, [world](http://example.com)!", writer);
            string plainText = writer.ToString();
            Assert.AreEqual("Hello, world!\n", plainText);
        }
    }
}
