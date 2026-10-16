using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

public class TestDefinitionLists
{
    [TestCase(":foo: Foo", 1)]
    [TestCase(":foo: Foo", 2)]
    [TestCase(":foo: Foo", 3)]
    [TestCase("~foo Foo", 1)]
    [TestCase("~foo Foo", 2)]
    [TestCase("~foo Foo", 3)]
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
        [Values(1, 2, 3)] int indent)
    {
        var markdown = $"{prefix}{new string(' ', indent)}{marker}";
        var pipeline = new MarkdownPipelineBuilder().UseDefinitionLists().UsePreciseSourceLocation().Build();
        var document = Markdown.Parse(markdown, pipeline);
        var literal = document.Descendants<LiteralInline>().Single(x => x.Content.ToString() == marker);

        Assert.AreEqual(indent, literal.Column);
        Assert.AreEqual(prefix.Length + indent, literal.Span.Start);
        Assert.AreEqual(markdown.Length - 1, literal.Span.End);
    }

    [Test]
    public void DefinitionMarkersAcceptSpacesOrTabs(
        [Values(':', '~')] char marker,
        [Values(" ", "  ", "\t")] string whitespace)
    {
        var markdown = $"Term\n{marker}{whitespace}First definition\n{marker}{whitespace}Second definition";
        const string expected = "<dl>\n<dt>Term</dt>\n<dd>First definition</dd>\n<dd>Second definition</dd>\n</dl>\n";

        TestParser.TestSpec(markdown, expected, "definitionlists+attributes|advanced");
    }

    // A definition renders its paragraphs the same way whether the enclosing list is tight or loose.
    [TestCase("- Term\n  :   first\n\n      next para",
        "<ul>\n<li>\n<dl>\n<dt>Term</dt>\n<dd><p>first</p>\n<p>next para</p>\n</dd>\n</dl>\n</li>\n</ul>\n")]
    [TestCase("- Term\n  :   first\n\n      next para\n\n- other",
        "<ul>\n<li>\n<dl>\n<dt>Term</dt>\n<dd><p>first</p>\n<p>next para</p>\n</dd>\n</dl>\n</li>\n<li><p>other</p>\n</li>\n</ul>\n")]
    [TestCase("> - Term\n>   :   first\n>\n>       next para",
        "<blockquote>\n<ul>\n<li>\n<dl>\n<dt>Term</dt>\n<dd><p>first</p>\n<p>next para</p>\n</dd>\n</dl>\n</li>\n</ul>\n</blockquote>\n")]
    [TestCase("- Term\n  : first\n    - a\n    - b",
        "<ul>\n<li>\n<dl>\n<dt>Term</dt>\n<dd><p>first</p>\n<ul>\n<li>a</li>\n<li>b</li>\n</ul>\n</dd>\n</dl>\n</li>\n</ul>\n")]
    [TestCase("- Term\n  :   simple\n  :   other",
        "<ul>\n<li>\n<dl>\n<dt>Term</dt>\n<dd>simple</dd>\n<dd>other</dd>\n</dl>\n</li>\n</ul>\n")]
    public void DefinitionParagraphsIgnoreEnclosingTightList(string markdown, string expected)
    {
        TestParser.TestSpec(markdown, expected, "definitionlists+attributes|advanced");
    }
}
