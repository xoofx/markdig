// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Collections.Generic;

namespace Markdig.Extensions.Globalization
{
    /// <summary>
    /// Extension to add support for RTL content.
    /// </summary>
    public class GlobalizationExtension : IMarkdownExtension
    {

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
                    attributes.AddPropertyIfNotExist("dir", "rtl");

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
                    // TaskList items contain an "X", which will cause
                    // the function to always return false.
                    if (child is TaskList)
                        continue;

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

        private bool StartsWithRtlCharacter(string text)
        {
            foreach (var c in CharHelper.ToUtf32(text))
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
