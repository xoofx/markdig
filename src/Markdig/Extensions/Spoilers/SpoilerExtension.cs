// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Spoilers;

/// <summary>
///     Extension for parsing inline spoilers starting and ending with an equal amount of pipe characters.
/// </summary>
/// <seealso cref="IMarkdownExtension"/>
public sealed class SpoilerExtension : IMarkdownExtension
{
    /// <inheritdoc />
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        SpoilerInlineParser? inlineParser = pipeline.InlineParsers.Find<SpoilerInlineParser>();
        if (inlineParser == null)
        {
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new SpoilerInlineParser());
        }
    }

    /// <inheritdoc />
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        SpoilerInlineRenderer? blockRenderer = renderer.ObjectRenderers.FindExact<SpoilerInlineRenderer>();
        if (blockRenderer == null)
        {
            renderer.ObjectRenderers.InsertBefore<QuoteBlockRenderer>(new SpoilerInlineRenderer());
        }
    }
}