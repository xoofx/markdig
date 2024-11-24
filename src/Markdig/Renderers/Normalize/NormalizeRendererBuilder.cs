// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.IO;

namespace Markdig.Renderers.Normalize;


/// <summary>
/// This class is used with <see cref="MarkdownExtensions.ConfigureNormalizeRenderer"/>
/// to set up a pipeline with a normalizing renderer.
/// </summary>
public class NormalizeRendererBuilder : IMarkdownRendererBuilder
{
    private readonly Lazy<NormalizeOptions> options = new(() => new NormalizeOptions());

    public NormalizeRenderer Build(TextWriter writer)
    {
        return new NormalizeRenderer(writer, options.IsValueCreated ? options.Value : null);
    }

    TextRendererBase IMarkdownRendererBuilder.Build(TextWriter writer) => Build(writer);

    /// <inheritdoc cref="NormalizeOptions.SpaceAfterQuoteBlock" />
    public NormalizeRendererBuilder UseSpaceAfterQuoteBlock(bool enable)
    {
        options.Value.SpaceAfterQuoteBlock = enable;
        return this;
    }

    /// <inheritdoc cref="NormalizeOptions.EmptyLineAfterCodeBlock" />
    public NormalizeRendererBuilder UseEmptyLineAfterCodeBlock(bool enable)
    {
        options.Value.EmptyLineAfterCodeBlock = enable;
        return this;
    }

    /// <inheritdoc cref="NormalizeOptions.EmptyLineAfterHeading" />
    public NormalizeRendererBuilder UseEmptyLineAfterHeading(bool enable)
    {
        options.Value.EmptyLineAfterHeading = enable;
        return this;
    }

    /// <inheritdoc cref="NormalizeOptions.EmptyLineAfterThematicBreak" />
    public NormalizeRendererBuilder UseEmptyLineAfterThematicBreak(bool enable)
    {
        options.Value.EmptyLineAfterThematicBreak = enable;
        return this;
    }

    /// <inheritdoc cref="NormalizeOptions.ListItemCharacter" />
    public NormalizeRendererBuilder UseListItemCharacter(char? character)
    {
        options.Value.ListItemCharacter = character;
        return this;
    }

    /// <inheritdoc cref="NormalizeOptions.ExpandAutoLinks" />
    public NormalizeRendererBuilder ExpandAutoLinks(bool enable)
    {
        options.Value.ExpandAutoLinks = enable;
        return this;
    }
}
