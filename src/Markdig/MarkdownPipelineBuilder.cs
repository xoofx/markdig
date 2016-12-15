// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.IO;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;

namespace Markdig
{
    /// <summary>
    /// This class allows to modify the pipeline to parse and render a Markdown document.
    /// </summary>
    /// <remarks>NOTE: A pipeline is not thread-safe.</remarks>
    public class MarkdownPipelineBuilder
    {
        private MarkdownPipeline pipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownPipeline" /> class.
        /// </summary>
        public MarkdownPipelineBuilder()
        {
            // Add all default parsers
            BlockParsers = new OrderedList<BlockParser>()
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

            InlineParsers = new OrderedList<InlineParser>()
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

            StringBuilderCache = new StringBuilderCache();
        }

        /// <summary>
        /// Gets the block parsers.
        /// </summary>
        public OrderedList<BlockParser> BlockParsers { get; private set; }

        /// <summary>
        /// Gets the inline parsers.
        /// </summary>
        public OrderedList<InlineParser> InlineParsers { get; private set; }

        /// <summary>
        /// Gets the register extensions.
        /// </summary>
        public OrderedList<IMarkdownExtension> Extensions { get; }

        /// <summary>
        /// Gets or sets the string builder cache used by the parsers.
        /// </summary>
        public StringBuilderCache StringBuilderCache { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable precise source location (slower parsing but accurate position for block and inline elements)
        /// </summary>
        public bool PreciseSourceLocation { get; set; }

        /// <summary>
        /// Gets or sets the debug log.
        /// </summary>
        public TextWriter DebugLog { get; set; }

        /// <summary>
        /// Occurs when a document has been processed after the <see cref="MarkdownParser.Parse"/> method.
        /// </summary>
        public event ProcessDocumentDelegate DocumentProcessed;

        internal ProcessDocumentDelegate GetDocumentProcessed => DocumentProcessed;

        /// <summary>
        /// Builds a pipeline from this instance. Once the pipeline is build, it cannot be modified.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">An extension cannot be null</exception>
        public MarkdownPipeline Build()
        {
            if (pipeline != null)
            {
                return pipeline;
            }

            // TODO: Review the whole initialization process for extensions
            // - It does not prevent a user to modify the pipeline after it has been used
            // - a pipeline is not thread safe.
            // We should find a proper way to make the pipeline safely modifiable/freezable (PipelineBuilder -> Pipeline)

            // Allow extensions to modify existing BlockParsers, InlineParsers and Renderer
            foreach (var extension in Extensions)
            {
                if (extension == null)
                {
                    throw new InvalidOperationException("An extension cannot be null");
                }
                extension.Setup(this);
            }

            pipeline = new MarkdownPipeline(new OrderedList<IMarkdownExtension>(Extensions),
                new BlockParserList(BlockParsers), new InlineParserList(InlineParsers), StringBuilderCache, DebugLog,
                GetDocumentProcessed) {PreciseSourceLocation = PreciseSourceLocation};
            return pipeline;
        }
    }
}