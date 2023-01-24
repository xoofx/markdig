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
}
