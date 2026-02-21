using Markdig.Syntax;

namespace Markdig.Tests;

public class TestContainerBlocks
{
    private class MockContainerBlock : ContainerBlock
    {
        public MockContainerBlock()
            : base(null)
        {

        }
    }

    [Test]
    public void CanBeCleared()
    {
        ContainerBlock container = new MockContainerBlock();
        Assert.AreEqual(0, container.Count);
        Assert.Null(container.LastChild);

        var paragraph = new ParagraphBlock();
        Assert.Null(paragraph.Parent);

        container.Add(paragraph);

        Assert.AreEqual(1, container.Count);
        Assert.AreSame(container, paragraph.Parent);
        Assert.AreSame(paragraph, container.LastChild);

        container.Clear();

        Assert.AreEqual(0, container.Count);
        Assert.Null(container.LastChild);
        Assert.Null(paragraph.Parent);
    }

    [Test]
    public void CanBeInsertedInto()
    {
        ContainerBlock container = new MockContainerBlock();

        var one = new ParagraphBlock();
        container.Insert(0, one);
        Assert.AreEqual(1, container.Count);
        Assert.AreSame(container[0], one);
        Assert.AreSame(container, one.Parent);

        var two = new ParagraphBlock();
        container.Insert(1, two);
        Assert.AreEqual(2, container.Count);
        Assert.AreSame(container[0], one);
        Assert.AreSame(container[1], two);
        Assert.AreSame(container, two.Parent);

        var three = new ParagraphBlock();
        container.Insert(0, three);
        Assert.AreEqual(3, container.Count);
        Assert.AreSame(container[0], three);
        Assert.AreSame(container[1], one);
        Assert.AreSame(container[2], two);
        Assert.AreSame(container, three.Parent);

        Assert.Throws<ArgumentNullException>(() => container.Insert(0, null));
        Assert.Throws<ArgumentOutOfRangeException>(() => container.Insert(4, new ParagraphBlock()));
        Assert.Throws<ArgumentOutOfRangeException>(() => container.Insert(-1, new ParagraphBlock()));
        Assert.Throws<ArgumentException>(() => container.Insert(0, one)); // one already has a parent
    }

    [Test]
    public void CanBeSet()
    {
        ContainerBlock container = new MockContainerBlock();

        var one = new ParagraphBlock();
        container.Insert(0, one);
        Assert.AreEqual(1, container.Count);
        Assert.AreSame(container[0], one);
        Assert.AreSame(container, one.Parent);

        var two = new ParagraphBlock();
        container[0] = two;
        Assert.AreSame(container, two.Parent);
        Assert.Null(one.Parent);

        Assert.Throws<ArgumentException>(() => container[0] = two); // two already has a parent
    }

    [Test]
    public void Contains()
    {
        var container = new MockContainerBlock();
        var block = new ParagraphBlock();

        Assert.False(container.Contains(block));

        container.Add(block);
        Assert.True(container.Contains(block));

        container.Add(new ParagraphBlock());
        Assert.True(container.Contains(block));

        container.Insert(0, new ParagraphBlock());
        Assert.True(container.Contains(block));
    }

    [Test]
    public void Remove()
    {
        var container = new MockContainerBlock();
        var block = new ParagraphBlock();

        Assert.False(container.Remove(block));
        Assert.AreEqual(0, container.Count);
        Assert.Throws<ArgumentOutOfRangeException>(() => container.RemoveAt(0));
        Assert.AreEqual(0, container.Count);

        container.Add(block);
        Assert.AreEqual(1, container.Count);
        Assert.True(container.Remove(block));
        Assert.AreEqual(0, container.Count);
        Assert.False(container.Remove(block));
        Assert.AreEqual(0, container.Count);

        container.Add(block);
        Assert.AreEqual(1, container.Count);
        container.RemoveAt(0);
        Assert.AreEqual(0, container.Count);
        Assert.Throws<ArgumentOutOfRangeException>(() => container.RemoveAt(0));
        Assert.AreEqual(0, container.Count);

        container.Add(new ParagraphBlock { Column = 1 });
        container.Add(new ParagraphBlock { Column = 2 });
        container.Add(new ParagraphBlock { Column = 3 });
        container.Add(new ParagraphBlock { Column = 4 });
        Assert.AreEqual(4, container.Count);

        container.RemoveAt(2);
        Assert.AreEqual(3, container.Count);
        Assert.AreEqual(4, container[2].Column);

        Assert.True(container.Remove(container[1]));
        Assert.AreEqual(2, container.Count);
        Assert.AreEqual(1, container[0].Column);
        Assert.AreEqual(4, container[1].Column);
        Assert.Throws<IndexOutOfRangeException>(() => _ = container[2]);
    }

    [Test]
    public void CopyTo()
    {
        var container = new MockContainerBlock();

        var destination = new Block[4];
        container.CopyTo(destination, 0);
        container.CopyTo(destination, 1);
        container.CopyTo(destination, -1);
        container.CopyTo(destination, 5);
        Assert.Null(destination[0]);

        container.Add(new ParagraphBlock());
        container.CopyTo(destination, 0);
        Assert.NotNull(destination[0]);
        Assert.Null(destination[1]);
        Assert.Null(destination[2]);
        Assert.Null(destination[3]);

        container.CopyTo(destination, 2);
        Assert.NotNull(destination[0]);
        Assert.Null(destination[1]);
        Assert.NotNull(destination[2]);
        Assert.Null(destination[3]);

        Array.Clear(destination);

        container.Add(new ParagraphBlock());
        container.CopyTo(destination, 1);
        Assert.Null(destination[0]);
        Assert.NotNull(destination[1]);
        Assert.NotNull(destination[2]);
        Assert.Null(destination[3]);

        Assert.Throws<IndexOutOfRangeException>(() => container.CopyTo(destination, 3));
    }

    [Test]
    public void CanTransferChildrenToAnotherContainer()
    {
        var source = new MockContainerBlock();
        var first = new ParagraphBlock { Column = 1 };
        var second = new ParagraphBlock { Column = 2 };
        source.Add(first);
        source.Add(second);

        var destination = new MockContainerBlock();
        var existing = new ParagraphBlock { Column = 0 };
        destination.Add(existing);

        source.TransferChildrenTo(destination);

        Assert.That(source.Count, Is.EqualTo(0));
        Assert.That(destination.Count, Is.EqualTo(3));
        Assert.That(destination[0], Is.SameAs(existing));
        Assert.That(destination[1], Is.SameAs(first));
        Assert.That(destination[2], Is.SameAs(second));
        Assert.That(first.Parent, Is.SameAs(destination));
        Assert.That(second.Parent, Is.SameAs(destination));
    }

    [Test]
    public void BlockCanBeRemovedAndReplaced()
    {
        var root = new MockContainerBlock();
        var toRemove = new ParagraphBlock();
        root.Add(toRemove);

        toRemove.Remove();
        Assert.That(root.Count, Is.EqualTo(0));
        Assert.That(toRemove.Parent, Is.Null);

        var sourceContainer = new MockContainerBlock();
        sourceContainer.Add(new ParagraphBlock { Column = 3 });
        sourceContainer.Add(new ParagraphBlock { Column = 4 });
        root.Add(sourceContainer);

        var replacement = new MockContainerBlock();
        sourceContainer.ReplaceBy(replacement);

        Assert.That(root[0], Is.SameAs(replacement));
        Assert.That(sourceContainer.Parent, Is.Null);
        Assert.That(sourceContainer.Count, Is.EqualTo(0));
        Assert.That(replacement.Count, Is.EqualTo(2));
        Assert.That(replacement[0].Column, Is.EqualTo(3));
        Assert.That(replacement[1].Column, Is.EqualTo(4));
    }
}
