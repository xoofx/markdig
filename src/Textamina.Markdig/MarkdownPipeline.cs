// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.IO;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig
{
    public class MarkdownPipeline
    {
        private BlockParserState blockParserState;
        private InlineParserState inlineParserState;
        private bool isInitialized;

        public MarkdownPipeline()
        {
            // Add all default parsers
            BlockParsers = new BlockParserList()
            {
                new ThematicBreakParser(),
                new HeadingBlockParser(),
                new QuoteBlockParser(),
                new ListBlockParser(),

                new HtmlBlockParser(),
                new FencedCodeBlockParser(),
                new IndentedCodeBlockParser(),
                new ParagraphBlockParser(),
            };

            InlineParsers = new InlineParserList()
            {
                new HtmlEntityParser(),
                new LinkInlineParser(),
                new EscapeInlineParser(),
                new EmphasisInlineParser(),
                new CodeInlineParser(),
                new AutolineInlineParser(),
                new LineBreakInlineParser(),
            };

            Extensions = new OrderedList<IMarkdownExtension>();

            Renderer = new HtmlRenderer(new StringWriter());

            StringBuilderCache = new StringBuilderCache();
        }

        public BlockParserList BlockParsers { get; private set; }

        public InlineParserList InlineParsers { get; private set; }

        public IMarkdownRenderer Renderer { get; set; }

        public OrderedList<IMarkdownExtension> Extensions { get; }

        public StringBuilderCache StringBuilderCache { get; set; }

        public void Initialize(out BlockParserState blockParserState, out InlineParserState inlineParserState)
        {
            blockParserState = this.blockParserState;
            inlineParserState = this.inlineParserState;
            if (isInitialized)
            {
                return;
            }

            // Make sure we have a StringBuilderCache by default
            StringBuilderCache = StringBuilderCache ?? new StringBuilderCache();

            // Allow extensions to modify existing BlockParsers, InlineParsers and Renderer
            foreach (var extension in Extensions)
            {
                if (extension == null)
                {
                    throw new InvalidOperationException("An extension cannot be null");
                }
                extension.Setup(this);
            }

            var document = new Document();

            // Make a copy of parsers
            var blockParserList = new BlockParserList();
            blockParserList.AddRange(BlockParsers);
            blockParserState =
                this.blockParserState = new BlockParserState(StringBuilderCache, document, blockParserList);

            var inlineParserList = new InlineParserList();
            inlineParserList.AddRange(InlineParsers);

            inlineParserState =
                this.inlineParserState = new InlineParserState(StringBuilderCache, document, InlineParsers);

            isInitialized = true;
        }
    }
}