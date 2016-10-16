// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.AutoIdentifiers
{
    /// <summary>
    /// The auto-identifier extension
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class AutoIdentifierExtension : IMarkdownExtension
    {
        private const string AutoIdentifierKey = "AutoIdentifier";
        private readonly HtmlRenderer stripRenderer;
        private readonly StringWriter headingWriter;
        private readonly AutoIdentifierOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoIdentifierExtension"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AutoIdentifierExtension(AutoIdentifierOptions options)
        {
            this.options = options;
            headingWriter = new StringWriter();
            // Use internally a HtmlRenderer to strip links from a heading
            stripRenderer = new HtmlRenderer(headingWriter)
            {
                // Set to false both to avoid having any HTML tags in the output
                EnableHtmlForInline = false,
                EnableHtmlEscape = false
            };
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
            if (headingBlockParser != null)
            {
                // Install a hook on the HeadingBlockParser when a HeadingBlock is actually processed
                headingBlockParser.Closed -= HeadingBlockParser_Closed;
                headingBlockParser.Closed += HeadingBlockParser_Closed;
            }
            var paragraphBlockParser = pipeline.BlockParsers.FindExact<ParagraphBlockParser>();
            if (paragraphBlockParser != null)
            {
                // Install a hook on the ParagraphBlockParser when a HeadingBlock is actually processed as a Setex heading
                paragraphBlockParser.Closed -= HeadingBlockParser_Closed;
                paragraphBlockParser.Closed += HeadingBlockParser_Closed;
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        /// <summary>
        /// Process on a new <see cref="HeadingBlock"/>
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="block">The heading block.</param>
        private void HeadingBlockParser_Closed(BlockProcessor processor, Block block)
        {
            // We may have a ParagraphBlock here as we have a hook on the ParagraphBlockParser
            var headingBlock = block as HeadingBlock;
            if (headingBlock == null)
            {
                return;
            }

            // If the AutoLink options is set, we register a LinkReferenceDefinition at the document level
            if ((options & AutoIdentifierOptions.AutoLink) != 0)
            {
                var headingLine = headingBlock.Lines.Lines[0];

                var text = headingLine.ToString();

                var linkRef = new HeadingLinkReferenceDefinition()
                {
                    Heading = headingBlock,
                    CreateLinkInline = CreateLinkInlineForHeading
                };
                processor.Document.SetLinkReferenceDefinition(text, linkRef);
            }

            // Then we register after inline have been processed to actually generate the proper #id
            headingBlock.ProcessInlinesEnd += HeadingBlock_ProcessInlinesEnd;
        }

        /// <summary>
        /// Callback when there is a reference to found to a heading. 
        /// Note that reference are only working if they are declared after.
        /// </summary>
        private Inline CreateLinkInlineForHeading(InlineProcessor inlineState, LinkReferenceDefinition linkRef, Inline child)
        {
            var headingRef = (HeadingLinkReferenceDefinition) linkRef;
            return new LinkInline()
            {
                // Use GetDynamicUrl to allow late binding of the Url (as a link may occur before the heading is declared and
                // the inlines of the heading are actually processed by HeadingBlock_ProcessInlinesEnd)
                GetDynamicUrl = () => HtmlHelper.Unescape("#" + headingRef.Heading.GetAttributes().Id),
                Title = HtmlHelper.Unescape(linkRef.Title),
            };
        }

        /// <summary>
        /// Process the inlines of the heading to create a unique identifier
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="inline">The inline.</param>
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

            // If id is already set, don't try to modify it
            var attributes = processor.Block.GetAttributes();
            if (attributes.Id != null)
            {
                return;
            }

            // Use a HtmlRenderer with 
            stripRenderer.Render(headingBlock.Inline);
            var headingText = headingWriter.ToString();
            headingWriter.GetStringBuilder().Length = 0;
            headingText = LinkHelper.Urilize(headingText, (options & AutoIdentifierOptions.AllowOnlyAscii) != 0);

            var baseHeadingId = string.IsNullOrEmpty(headingText) ? "section" : headingText;
            int index = 0;
            var headingId = baseHeadingId;
            var headingBuffer = StringBuilderCache.Local();
            while (!identifiers.Add(headingId))
            {
                index++;
                headingBuffer.Append(baseHeadingId);
                headingBuffer.Append('-');
                headingBuffer.Append(index);
                headingId = headingBuffer.ToString();
                headingBuffer.Length = 0;
            }

            attributes.Id = headingId;
        }
    }
}