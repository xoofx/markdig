using System;
using Markdig.Syntax;
using NUnit.Framework;

namespace Markdig.Tests
{
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
        public void CopyToCopiesChildren()
        {
            ContainerBlock container = new MockContainerBlock();
            var one = new ParagraphBlock();
            container.Add(one);
            var two = new ParagraphBlock();
            container.Add(two);
            var three = new ParagraphBlock();
            container.Add(three);
            Assert.AreEqual(3, container.Count);

            var destination = new Block[3];
            container.CopyTo(destination, 0);

            Assert.AreSame(destination[0], one);
            Assert.AreSame(destination[1], two);
            Assert.AreSame(destination[2], three);
        }
    }
}
