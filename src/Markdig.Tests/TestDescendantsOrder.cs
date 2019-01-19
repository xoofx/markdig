using NUnit.Framework;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
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
            foreach (var markdown in TestParser.SpecsMarkdown)
            {
                AssertSameDescendantsOrder(markdown);
            }
        }

        private void AssertSameDescendantsOrder(string markdown)
        {
            var syntaxTree = Markdown.Parse(markdown, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

            var descendants_legacy = Descendants_Legacy(syntaxTree).ToList();
            var descendants_new = syntaxTree.Descendants().ToList();

            Assert.AreEqual(descendants_legacy.Count, descendants_new.Count);

            for (int i = 0; i < descendants_legacy.Count; i++)
            {
                Assert.AreSame(descendants_legacy[i], descendants_new[i]);
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
