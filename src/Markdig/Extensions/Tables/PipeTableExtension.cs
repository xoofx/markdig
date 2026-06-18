// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Emoji;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;

namespace Markdig.Extensions.Tables;

/// <summary>
/// Extension that allows to use pipe tables.
/// </summary>
/// <seealso cref="IMarkdownExtension" />
public class PipeTableExtension : IMarkdownExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PipeTableExtension"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public PipeTableExtension(PipeTableOptions? options = null)
    {
        Options = options ?? new PipeTableOptions();
    }

    /// <summary>
    /// Gets the options.
    /// </summary>
    public PipeTableOptions Options { get; }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
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
            // Pipe table post-processing must split the paragraph into isolated table cells before emphasis
            // post-processing can pair delimiters across the paragraph. Otherwise an unmatched emphasis-extra
            // delimiter inside a cell, e.g. the subscript `~` in `**~$1.50**`, can leave pipe delimiters nested
            // in an emphasis delimiter tree and make the table parser see phantom cells. PipeTableParser re-runs
            // inline post-processors inside each extracted cell after the table structure has been fixed.
            var pipeTableParser = new PipeTableParser(lineBreakParser!, Options);
            if (!pipeline.InlineParsers.InsertBefore<EmojiParser>(pipeTableParser))
            {
                pipeline.InlineParsers.InsertBefore<EmphasisInlineParser>(pipeTableParser);
            }
        }
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer && !htmlRenderer.ObjectRenderers.Contains<HtmlTableRenderer>())
        {
            htmlRenderer.ObjectRenderers.Add(new HtmlTableRenderer());
        }
    }
}
