// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.GenericAttributes
{
    /// <summary>
    /// Extension that allows to attach HTML attributes to the previous <see cref="Inline"/> or current <see cref="Block"/>.
    /// This extension should be enabled last after enabling other extensions.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class GenericAttributesExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<GenericAttributesParser>())
            {
                pipeline.InlineParsers.Insert(0, new GenericAttributesParser());
            }

            // Plug into all IAttributesParseable
            foreach (var parser in pipeline.BlockParsers)
            {
                var attributesParseable = parser as IAttributesParseable;
                if (attributesParseable != null)
                {
                    attributesParseable.TryParseAttributes = TryProcessAttributesForHeading;
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        private bool TryProcessAttributesForHeading(BlockProcessor processor, ref StringSlice line, IBlock block)
        {
            // Try to find if there is any attributes { in the info string on the first line of a FencedCodeBlock
            if (line.Start < line.End)
            {
                int indexOfAttributes = line.IndexOf('{');
                if (indexOfAttributes >= 0)
                {
                    // Work on a copy
                    var copy = line;
                    copy.Start = indexOfAttributes;
                    var startOfAttributes = copy.Start;
                    HtmlAttributes attributes;
                    if (GenericAttributesParser.TryParse(ref copy, out attributes))
                    {
                        var htmlAttributes = block.GetAttributes();
                        attributes.CopyTo(htmlAttributes);

                        // Update position for HtmlAttributes
                        htmlAttributes.Line = processor.LineIndex;
                        htmlAttributes.Column = startOfAttributes - processor.CurrentLineStartPosition; // This is not accurate with tabs!
                        htmlAttributes.Span.Start = startOfAttributes;
                        htmlAttributes.Span.End = copy.Start - 1;

                        line.End = indexOfAttributes - 1;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}