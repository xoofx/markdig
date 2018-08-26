using Markdig.Extensions.Tables;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Markdig.Extensions.Globalization
{
    /// <summary>
    /// Extension to add support for RTL content.
    /// </summary>
    public class GlobalizationExtension : IMarkdownExtension
    {
        private const string Direction = "dir";
        private const string RightToLeft = "rtl";

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Make sure we don't have a delegate twice
            pipeline.DocumentProcessed -= Pipeline_DocumentProcessed;
            pipeline.DocumentProcessed += Pipeline_DocumentProcessed;
        }

        private void Pipeline_DocumentProcessed(MarkdownDocument document)
        {
            foreach (var node in document.Descendants())
            {
                if (node is TableRow || node is TableCell || node is ListItemBlock)
                    continue;

                if (ShouldBeRightToLeft(node))
                {
                    var attributes = node.GetAttributes();
                    attributes.AddPropertyIfNotExist(Direction, RightToLeft);

                    if (node is Table table)
                    {
                        attributes.AddPropertyIfNotExist("align", "right");
                    }
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {

        }

        private bool ShouldBeRightToLeft(MarkdownObject item)
        {
            if (item is IEnumerable<MarkdownObject> container)
            {
                foreach (var child in container)
                {
                    return ShouldBeRightToLeft(child);
                }
            }
            else if (item is LeafBlock leaf)
            {
                return ShouldBeRightToLeft(leaf.Inline);
            }
            else if (item is LiteralInline literal)
            {
                return StartsWithRtlCharacter(literal.ToString());
            }

            foreach (var descendant in item.Descendants())
            {
                if (descendant is ParagraphBlock p)
                {
                    foreach (var i in p.Inline)
                    {
                        if (i is LiteralInline l)
                        {
                            return StartsWithRtlCharacter(l.ToString());
                        }
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        private bool StartsWithRtlCharacter(string text)
        {
            foreach (var c in text)
            {
                if (CharHelper.IsRightToLeft(c))
                    return true;
                else if (CharHelper.IsLeftToRight(c))
                    return false;
            }

            return false;
        }
    }
}
