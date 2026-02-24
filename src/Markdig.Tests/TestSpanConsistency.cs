using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestSpanConsistency
{
    private sealed class MockContainerBlock : ContainerBlock
    {
        public MockContainerBlock() : base(null)
        {
        }
    }

    [Test]
    public void BlockUpdateSpanToIncludePropagatesToParents()
    {
        var root = new MockContainerBlock { Span = new SourceSpan(30, 40) };
        var child = new MockContainerBlock { Span = new SourceSpan(32, 35) };
        root.Add(child);

        child.UpdateSpanToInclude(new SourceSpan(10, 50));

        Assert.That(child.Span, Is.EqualTo(new SourceSpan(10, 50)));
        Assert.That(root.Span, Is.EqualTo(new SourceSpan(10, 50)));
    }

    [Test]
    public void ContainerBlockCanValidateAndUpdateSpansRecursively()
    {
        var document = new MarkdownDocument();
        var container = new MockContainerBlock { Span = new SourceSpan(30, 31) };
        var paragraph = new ParagraphBlock { Span = new SourceSpan(23, 24) };
        paragraph.Inline = new ContainerInline { Span = new SourceSpan(22, 24) };

        container.Add(paragraph);
        document.Add(container);
        document.Span = new SourceSpan(0, 5);

        Assert.That(document.HasValidSpan(recursive: true), Is.False);

        Assert.That(document.UpdateSpanFromChildren(recursive: true), Is.True);
        Assert.That(document.HasValidSpan(recursive: true), Is.True);
        Assert.That(paragraph.Span, Is.EqualTo(new SourceSpan(22, 24)));
        Assert.That(container.Span, Is.EqualTo(new SourceSpan(22, 31)));
        Assert.That(document.Span, Is.EqualTo(new SourceSpan(0, 31)));

        container.Span = new SourceSpan(0, 100);
        document.Span = new SourceSpan(0, 100);

        Assert.That(document.UpdateSpanFromChildren(recursive: true, preserveSelfSpan: false), Is.True);
        Assert.That(container.Span, Is.EqualTo(new SourceSpan(22, 24)));
        Assert.That(document.Span, Is.EqualTo(new SourceSpan(22, 24)));
    }

    [Test]
    public void ContainerInlineCanValidateAndUpdateSpansRecursively()
    {
        var root = new ContainerInline { Span = new SourceSpan(35, 36) };
        var nested = new EmphasisInline { Span = new SourceSpan(50, 51) };
        var literal = new LiteralInline("x") { Span = new SourceSpan(10, 12) };

        nested.AppendChild(literal);
        root.AppendChild(nested);

        Assert.That(root.HasValidSpan(recursive: true), Is.False);

        Assert.That(root.UpdateSpanFromChildren(recursive: true), Is.True);
        Assert.That(root.HasValidSpan(recursive: true), Is.True);
        Assert.That(nested.Span, Is.EqualTo(new SourceSpan(10, 51)));
        Assert.That(root.Span, Is.EqualTo(new SourceSpan(10, 51)));

        nested.Span = new SourceSpan(0, 100);
        root.Span = new SourceSpan(0, 100);

        Assert.That(root.UpdateSpanFromChildren(recursive: true, preserveSelfSpan: false), Is.True);
        Assert.That(nested.Span, Is.EqualTo(new SourceSpan(10, 12)));
        Assert.That(root.Span, Is.EqualTo(new SourceSpan(10, 12)));
    }

    [Test]
    public void ContainerBlockSpanCanBeExpandedAfterInsertions()
    {
        var container = new MockContainerBlock { Span = new SourceSpan(20, 25) };
        container.Insert(0, new ParagraphBlock { Span = new SourceSpan(10, 12) });
        container.Insert(1, new ParagraphBlock { Span = new SourceSpan(30, 35) });

        Assert.That(container.HasValidSpan(), Is.False);
        Assert.That(container.UpdateSpanFromChildren(), Is.True);
        Assert.That(container.Span, Is.EqualTo(new SourceSpan(10, 35)));
    }
}
