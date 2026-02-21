using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

public class TestContainerInlines
{
    private class MockLeafBlock : LeafBlock
    {
        public MockLeafBlock()
            : base(null)
        {

        }
    }

    [Test]
    public void CanBeAddedToLeafBlock()
    {
        var leafBlock1 = new MockLeafBlock();

        var one = new ContainerInline();
        Assert.Null(one.ParentBlock);

        leafBlock1.Inline = one;
        Assert.AreSame(leafBlock1, one.ParentBlock);

        var two = new ContainerInline();
        Assert.Null(two.ParentBlock);

        leafBlock1.Inline = two;
        Assert.AreSame(leafBlock1, two.ParentBlock);
        Assert.Null(one.ParentBlock);

        var leafBlock2 = new MockLeafBlock();
        Assert.Throws<ArgumentException>(() => leafBlock2.Inline = two);
    }

    [Test]
    public void CanTransferChildrenToAnotherContainer()
    {
        var source = new ContainerInline();
        var first = new LiteralInline("a");
        var second = new LiteralInline("b");
        source.AppendChild(first);
        source.AppendChild(second);

        var destination = new ContainerInline();
        var existing = new LiteralInline("x");
        destination.AppendChild(existing);

        source.TransferChildrenTo(destination);

        Assert.That(source.FirstChild, Is.Null);
        Assert.That(source.LastChild, Is.Null);
        Assert.That(destination.FirstChild, Is.SameAs(existing));
        Assert.That(existing.NextSibling, Is.SameAs(first));
        Assert.That(first.NextSibling, Is.SameAs(second));
        Assert.That(second.NextSibling, Is.Null);
        Assert.That(first.Parent, Is.SameAs(destination));
        Assert.That(second.Parent, Is.SameAs(destination));
    }
}
