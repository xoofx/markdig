using NUnit.Framework;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestDescendantsOrder
    {
        [Test]
        public void TestSchemas()
        {
            foreach (var syntaxTree in TestParser.SpecsSyntaxTrees)
            {
                AssertIEnumerablesAreEqual(
                    Descendants_Legacy(syntaxTree),
                    syntaxTree.Descendants());

                AssertIEnumerablesAreEqual(
                    syntaxTree.Descendants().OfType<ParagraphBlock>(),
                    syntaxTree.Descendants<ParagraphBlock>());

                AssertIEnumerablesAreEqual(
                    syntaxTree.Descendants().OfType<ParagraphBlock>(),
                    (syntaxTree as ContainerBlock).Descendants<ParagraphBlock>());

                AssertIEnumerablesAreEqual(
                    syntaxTree.Descendants().OfType<LiteralInline>(),
                    syntaxTree.Descendants<LiteralInline>());

                foreach (LiteralInline literalInline in syntaxTree.Descendants<LiteralInline>())
                {
                    Assert.AreSame(Array.Empty<ListBlock>(), literalInline.Descendants<ListBlock>());
                    Assert.AreSame(Array.Empty<ParagraphBlock>(), literalInline.Descendants<ParagraphBlock>());
                    Assert.AreSame(Array.Empty<ContainerInline>(), literalInline.Descendants<ContainerInline>());
                }

                foreach (ContainerInline containerInline in syntaxTree.Descendants<ContainerInline>())
                {
                    AssertIEnumerablesAreEqual(
                        containerInline.FindDescendants<LiteralInline>(),
                        containerInline.Descendants<LiteralInline>());

                    AssertIEnumerablesAreEqual(
                        containerInline.FindDescendants<LiteralInline>(),
                        (containerInline as MarkdownObject).Descendants<LiteralInline>());

                    if (containerInline.FirstChild is null)
                    {
                        Assert.AreSame(Array.Empty<LiteralInline>(), containerInline.Descendants<LiteralInline>());
                        Assert.AreSame(Array.Empty<LiteralInline>(), containerInline.FindDescendants<LiteralInline>());
                        Assert.AreSame(Array.Empty<LiteralInline>(), (containerInline as MarkdownObject).Descendants<LiteralInline>());
                    }

                    Assert.AreSame(Array.Empty<ListBlock>(), containerInline.Descendants<ListBlock>());
                    Assert.AreSame(Array.Empty<ParagraphBlock>(), containerInline.Descendants<ParagraphBlock>());
                }

                foreach (ParagraphBlock paragraphBlock in syntaxTree.Descendants<ParagraphBlock>())
                {
                    AssertIEnumerablesAreEqual(
                        (paragraphBlock as MarkdownObject).Descendants<LiteralInline>(),
                        paragraphBlock.Descendants<LiteralInline>());

                    Assert.AreSame(Array.Empty<ParagraphBlock>(), paragraphBlock.Descendants<ParagraphBlock>());
                }

                foreach (ContainerBlock containerBlock in syntaxTree.Descendants<ContainerBlock>())
                {
                    AssertIEnumerablesAreEqual(
                        containerBlock.Descendants<LiteralInline>(),
                        (containerBlock as MarkdownObject).Descendants<LiteralInline>());

                    AssertIEnumerablesAreEqual(
                        containerBlock.Descendants<ParagraphBlock>(),
                        (containerBlock as MarkdownObject).Descendants<ParagraphBlock>());

                    if (containerBlock.Count == 0)
                    {
                        Assert.AreSame(Array.Empty<LiteralInline>(), containerBlock.Descendants<LiteralInline>());
                        Assert.AreSame(Array.Empty<LiteralInline>(), (containerBlock as Block).Descendants<LiteralInline>());
                        Assert.AreSame(Array.Empty<LiteralInline>(), (containerBlock as MarkdownObject).Descendants<LiteralInline>());
                    }
                }
            }
        }

        private static void AssertIEnumerablesAreEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstList = new List<T>(first);
            var secondList = new List<T>(second);

            Assert.AreEqual(firstList.Count, secondList.Count);

            for (int i = 0; i < firstList.Count; i++)
            {
                Assert.AreSame(firstList[i], secondList[i]);
            }
        }

        private static IEnumerable<MarkdownObject> Descendants_Legacy(MarkdownObject markdownObject)
        {
            // TODO: implement a recursiveless method

            var block = markdownObject as ContainerBlock;
            if (block != null)
            {
                foreach (var subBlock in block)
                {
                    yield return subBlock;

                    foreach (var sub in Descendants_Legacy(subBlock))
                    {
                        yield return sub;
                    }

                    // Visit leaf block that have inlines
                    var leafBlock = subBlock as LeafBlock;
                    if (leafBlock?.Inline != null)
                    {
                        foreach (var subInline in Descendants_Legacy(leafBlock.Inline))
                        {
                            yield return subInline;
                        }
                    }
                }
            }
            else
            {
                var inline = markdownObject as ContainerInline;
                if (inline != null)
                {
                    var child = inline.FirstChild;
                    while (child != null)
                    {
                        var next = child.NextSibling;
                        yield return child;

                        foreach (var sub in Descendants_Legacy(child))
                        {
                            yield return sub;
                        }

                        child = next;
                    }
                }
            }
        }
    }
}
