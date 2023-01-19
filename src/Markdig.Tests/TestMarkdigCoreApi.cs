using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

public class TestMarkdigCoreApi
{
    [Test]
    public void TestToHtml()
    {
        for (int i = 0; i < 5; i++)
        {
            string html = Markdown.ToHtml("This is a text with some *emphasis*");
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);

            html = Markdown.ToHtml("This is a text with a https://link.tld/");
            Assert.AreNotEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
        }
    }

    [Test]
    public void TestToHtmlWithPipeline()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .Build();

        for (int i = 0; i < 5; i++)
        {
            string html = Markdown.ToHtml("This is a text with some *emphasis*", pipeline);
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);

            html = Markdown.ToHtml("This is a text with a https://link.tld/", pipeline);
            Assert.AreNotEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
        }

        pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        for (int i = 0; i < 5; i++)
        {
            string html = Markdown.ToHtml("This is a text with a https://link.tld/", pipeline);
            Assert.AreEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
        }
    }

    [Test]
    public void TestToHtmlWithWriter()
    {
        var writer = new StringWriter();

        for (int i = 0; i < 5; i++)
        {
            _ = Markdown.ToHtml("This is a text with some *emphasis*", writer);
            string html = writer.ToString();
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);
            writer.GetStringBuilder().Length = 0;
        }

        writer = new StringWriter();
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        for (int i = 0; i < 5; i++)
        {
            _ = Markdown.ToHtml("This is a text with a https://link.tld/", writer, pipeline);
            string html = writer.ToString();
            Assert.AreEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
            writer.GetStringBuilder().Length = 0;
        }
    }

    [Test]
    public void TestDocumentToHtmlWithWriter()
    {
        var writer = new StringWriter();

        for (int i = 0; i < 5; i++)
        {
            MarkdownDocument document = Markdown.Parse("This is a text with some *emphasis*");
            document.ToHtml(writer);
            string html = writer.ToString();
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);
            writer.GetStringBuilder().Length = 0;
        }

        writer = new StringWriter();
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        for (int i = 0; i < 5; i++)
        {
            MarkdownDocument document = Markdown.Parse("This is a text with a https://link.tld/", pipeline);
            document.ToHtml(writer, pipeline);
            string html = writer.ToString();
            Assert.AreEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
            writer.GetStringBuilder().Length = 0;
        }
    }

    [Test]
    public void TestConvert()
    {
        var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);

        for (int i = 0; i < 5; i++)
        {
            _ = Markdown.Convert("This is a text with some *emphasis*", renderer);
            string html = writer.ToString();
            Assert.AreEqual("<p>This is a text with some <em>emphasis</em></p>\n", html);
            writer.GetStringBuilder().Length = 0;
        }

        writer = new StringWriter();
        renderer = new HtmlRenderer(writer);
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        for (int i = 0; i < 5; i++)
        {
            _ = Markdown.Convert("This is a text with a https://link.tld/", renderer, pipeline);
            string html = writer.ToString();
            Assert.AreEqual("<p>This is a text with a <a href=\"https://link.tld/\">https://link.tld/</a></p>\n", html);
            writer.GetStringBuilder().Length = 0;
        }
    }

    [Test]
    public void TestParse()
    {
        const string markdown = "This is a text with some *emphasis*";

        var pipeline = new MarkdownPipelineBuilder()
            .UsePreciseSourceLocation()
            .Build();

        for (int i = 0; i < 5; i++)
        {
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
    }

    [Test]
    public void TestNormalize()
    {
        for (int i = 0; i < 5; i++)
        {
            string normalized = Markdown.Normalize("Heading\n=======");
            Assert.AreEqual("# Heading", normalized);
        }
    }

    [Test]
    public void TestNormalizeWithWriter()
    {
        for (int i = 0; i < 5; i++)
        {
            var writer = new StringWriter();

            _ = Markdown.Normalize("Heading\n=======", writer);
            string normalized = writer.ToString();
            Assert.AreEqual("# Heading", normalized);
        }
    }

    [Test]
    public void TestToPlainText()
    {
        for (int i = 0; i < 5; i++)
        {
            string plainText = Markdown.ToPlainText("*Hello*, [world](http://example.com)!");
            Assert.AreEqual("Hello, world!\n", plainText);
        }
    }

    [Test]
    public void TestToPlainTextWithWriter()
    {
        for (int i = 0; i < 5; i++)
        {
            var writer = new StringWriter();

            _ = Markdown.ToPlainText("*Hello*, [world](http://example.com)!", writer);
            string plainText = writer.ToString();
            Assert.AreEqual("Hello, world!\n", plainText);
        }
    }
}
