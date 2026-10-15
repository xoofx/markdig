using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

public class TestDefinitionLists
{
    [TestCase(":foo: Foo", 1)]
    [TestCase(":foo: Foo", 2)]
    [TestCase("~foo Foo", 1)]
    [TestCase("~foo Foo", 2)]
    public void IndentedInvalidContinuationPreservesTerm(string term, int indent)
    {
        var markdown = $"Term\n:   First definition\n\n{new string(' ', indent)}{term}\n:   Second definition";
        var expected = $"<dl>\n<dt>Term</dt>\n<dd>First definition</dd>\n<dt>{term}</dt>\n<dd>Second definition</dd>\n</dl>\n";

        TestParser.TestSpec(markdown, expected, "definitionlists+attributes|advanced");
    }

    [Test]
    public void InvalidDefinitionMarkerPreservesSourceLocation(
        [Values("Term\n", "Term\n:   First definition\n")] string prefix,
        [Values(":foo: Foo", "~foo Foo")] string marker,
        [Values(1, 2)] int indent)
    {
        var markdown = $"{prefix}{new string(' ', indent)}{marker}";
        var pipeline = new MarkdownPipelineBuilder().UseDefinitionLists().UsePreciseSourceLocation().Build();
        var document = Markdown.Parse(markdown, pipeline);
        var literal = document.Descendants<LiteralInline>().Single(x => x.Content.ToString() == marker);

        Assert.AreEqual(indent, literal.Column);
        Assert.AreEqual(prefix.Length + indent, literal.Span.Start);
        Assert.AreEqual(markdown.Length - 1, literal.Span.End);
    }
}
