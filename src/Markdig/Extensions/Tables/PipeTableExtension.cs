// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers.Inlines;
using Markdig.Renderers;

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// Extension that allows to use pipe tables.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class PipeTableExtension : IMarkdownExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTableExtension"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public PipeTableExtension(PipeTableOptions options = null)
        {
            Options = options ?? new PipeTableOptions();
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public PipeTableOptions Options { get; }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Pipe tables require precise source location
            pipeline.PreciseSourceLocation = true;
            if (!pipeline.BlockParsers.Contains<PipeTableBlockParser>())
            {
                pipeline.BlockParsers.Insert(0, new PipeTableBlockParser());
            }
            var lineBreakParser = pipeline.InlineParsers.FindExact<LineBreakInlineParser>();
            if (!pipeline.InlineParsers.Contains<PipeTableParser>())
            {
                pipeline.InlineParsers.InsertBefore<EmphasisInlineParser>(new PipeTableParser(lineBreakParser, Options));
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlTableRenderer>())
            {
                htmlRenderer.ObjectRenderers.Add(new HtmlTableRenderer());
            }
        }
    }
}