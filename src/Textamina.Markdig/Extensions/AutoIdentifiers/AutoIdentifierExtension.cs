// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.AutoIdentifiers
{
    public class AutoIdentifierExtension : IMarkdownExtension
    {
        private const string AutoIdentifierKey = "AutoIdentifier";
        private readonly HtmlRenderer stripRenderer;
        private readonly StringWriter headingWriter;

        public AutoIdentifierExtension()
        {
            headingWriter = new StringWriter();
            stripRenderer = new HtmlRenderer(headingWriter)
            {
                EnableHtmlForInline = false,
                EnableHtmlEscape = false
            };
        }

        public void Setup(MarkdownPipeline pipeline)
        {
            var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
            if (headingBlockParser != null)
            {
                headingBlockParser.Closed -= HeadingBlockParser_Closed;
                headingBlockParser.Closed += HeadingBlockParser_Closed;
            }
            var paragraphBlockParser = pipeline.BlockParsers.Find<ParagraphBlockParser>();
            if (paragraphBlockParser != null)
            {
                paragraphBlockParser.Closed -= HeadingBlockParser_Closed;
                paragraphBlockParser.Closed += HeadingBlockParser_Closed;
            }
        }

        private void HeadingBlockParser_Closed(BlockProcessor processor, Block block)
        {
            var headingBlock = block as HeadingBlock;
            if (headingBlock == null)
            {
                return;
            }
            headingBlock.ProcessInlinesEnd += HeadingBlock_ProcessInlinesEnd;
        }

        private void HeadingBlock_ProcessInlinesEnd(InlineProcessor processor, Inline inline)
        {
            var identifiers = processor.Document.GetData(AutoIdentifierKey) as HashSet<string>;
            if (identifiers == null)
            {
                identifiers = new HashSet<string>();
                processor.Document.SetData(AutoIdentifierKey, identifiers);
            }

            var headingBlock = (HeadingBlock) processor.Block;
            if (headingBlock.Inline == null)
            {
                return;
            }

            stripRenderer.Render(headingBlock.Inline);
            var headingText = headingWriter.ToString();
            headingWriter.GetStringBuilder().Length = 0;

            var headingBuffer = StringBuilderCache.Local();
            bool hasLetter = false;
            bool previousIsSpace = false;
            for (int i = 0; i < headingText.Length; i++)
            {
                var c = headingText[i];
                if (char.IsLetter(c))
                {
                    c = char.IsUpper(c) ? char.ToLowerInvariant(c) : c;
                    headingBuffer.Append(c);
                    hasLetter = true;
                    previousIsSpace = false;
                }
                else if (hasLetter)
                {
                    switch (c)
                    {
                        case '_':
                        case '-':
                        case '.':
                            if (headingBuffer[headingBuffer.Length - 1] != c)
                            {
                                if (previousIsSpace)
                                {
                                    headingBuffer.Length--;
                                    previousIsSpace = false;
                                }
                                headingBuffer.Append(c);
                            }
                            previousIsSpace = false;
                            break;
                        default:
                            if (!previousIsSpace && c.IsWhitespace())
                            {
                                var pc = headingBuffer[headingBuffer.Length - 1];
                                if (!IsReservedPunctuation(pc))
                                {
                                    headingBuffer.Append('-');
                                }
                                previousIsSpace = true;
                            }
                            break;
                    }
                }
            }

            // Trim trailing _ - .
            while (headingBuffer.Length > 0)
            {
                var c = headingBuffer[headingBuffer.Length - 1];
                if (IsReservedPunctuation(c))
                {
                    headingBuffer.Length--;
                }
                else
                {
                    break;
                }
            }

            var baseHeadingId = headingBuffer.Length == 0 ? "section" : headingBuffer.ToString();
            headingBuffer.Length = 0;
            int index = 0;
            var headingId = baseHeadingId;
            while (!identifiers.Add(headingId))
            {
                index++;
                headingBuffer.Append(baseHeadingId);
                headingBuffer.Append('-');
                headingBuffer.Append(index);
                headingId = headingBuffer.ToString();
                headingBuffer.Length = 0;
            }

            processor.Block.GetAttributes().Id = headingId;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        private static bool IsReservedPunctuation(char c)
        {
            return c == '_' || c == '-' || c == '.';
        }
    }
}