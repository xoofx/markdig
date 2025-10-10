// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;

namespace Markdig.Extensions.HtmlTagFilter;

/// <summary>
/// Extension to filter HTML tags based on whitelist or blacklist.
/// </summary>
public class HtmlTagFilterExtension : IMarkdownExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlTagFilterExtension"/> class.
    /// </summary>
    /// <param name="options">The filter options.</param>
    public HtmlTagFilterExtension(HtmlTagFilterOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Gets the filter options.
    /// </summary>
    public HtmlTagFilterOptions Options { get; }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Replace HtmlBlockParser with filtered version
        var htmlBlockParser = pipeline.BlockParsers.Find<HtmlBlockParser>();
        if (htmlBlockParser != null)
        {
            var index = pipeline.BlockParsers.IndexOf(htmlBlockParser);
            pipeline.BlockParsers.RemoveAt(index);
            pipeline.BlockParsers.Insert(index, new FilteredHtmlBlockParser(Options));
        }

        // Replace AutolinkInlineParser with filtered version
        var autolinkParser = pipeline.InlineParsers.Find<AutolinkInlineParser>();
        if (autolinkParser != null)
        {
            var index = pipeline.InlineParsers.IndexOf(autolinkParser);
            pipeline.InlineParsers.RemoveAt(index);
            pipeline.InlineParsers.Insert(index, new FilteredAutolinkInlineParser(autolinkParser.Options, Options));
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        // No renderer setup needed
    }
}
