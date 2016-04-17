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
    public class MarkdownPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownPipeline" /> class.
        /// </summary>
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

        /// <summary>
        /// Gets the block parsers.
        /// </summary>
        public BlockParserList BlockParsers { get; private set; }

        /// <summary>
        /// Gets the inline parsers.
        /// </summary>
        public InlineParserList InlineParsers { get; private set; }

        /// <summary>
        /// Gets or sets the renderer.
        /// </summary>
        public IMarkdownRenderer Renderer { get; set; }

        /// <summary>
        /// Gets the register extensions.
        /// </summary>
        public OrderedList<IMarkdownExtension> Extensions { get; }

        /// <summary>
        /// Gets or sets the string builder cache used by the parsers.
        /// </summary>
        public StringBuilderCache StringBuilderCache { get; set; }

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
        /// Initializes this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">An extension cannot be null</exception>
        public void Initialize()
        {
            // Allow extensions to modify existing BlockParsers, InlineParsers and Renderer
            foreach (var extension in Extensions)
            {
                if (extension == null)
                {
                    throw new InvalidOperationException("An extension cannot be null");
                }
                extension.Setup(this);
            }
        }
    }
}