// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.PragmaLines
{
    /// <summary>
    /// Extension to a span for each line containing the original line id (using id = pragma-line#line_number_zero_based)
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class PragmaLineExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.DocumentProcessed -= PipelineOnDocumentProcessed;
            pipeline.DocumentProcessed += PipelineOnDocumentProcessed;
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        private static void PipelineOnDocumentProcessed(MarkdownDocument document)
        {
            int index = 0;
            AddPragmas(document, ref index);
        }

        private static void AddPragmas(Block block, ref int index)
        {
            var attribute = block.GetAttributes();
            var pragmaId = GetPragmaId(block);
            if ( attribute.Id == null)
            {
                attribute.Id = pragmaId;
            }
            else if (block.Parent != null)
            {
                var heading = block as HeadingBlock;

                // If we have a heading, we will try to add the tag inside it
                // otherwise we will add it just before
                var tag = $"<a id=\"{pragmaId}\"></a>";
                if (heading?.Inline?.FirstChild != null)
                {
                    heading.Inline.FirstChild.InsertBefore(new HtmlInline() { Tag = tag });

                }
                else
                {
                    block.Parent.Insert(index, new HtmlBlock(null) { Lines = new StringLineGroup(tag) });
                    index++;
                }
            }

            var container = block as ContainerBlock;
            if (container != null)
            {
                for (int i = 0; i < container.Count; i++)
                {
                    var subBlock = container[i];
                    AddPragmas(subBlock, ref i);
                }
            }
        }

        private static string GetPragmaId(Block block)
        {
            return $"pragma-line-{block.Line}";
        }
    }
}