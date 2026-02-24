// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Roundtrip;

namespace Markdig.Extensions.Yaml;

/// <summary>
/// Represents the YamlFrontMatterRoundtripRenderer type.
/// </summary>
public class YamlFrontMatterRoundtripRenderer : MarkdownObjectRenderer<RoundtripRenderer, YamlFrontMatterBlock>
{
    private readonly CodeBlockRenderer _codeBlockRenderer;

    /// <summary>
    /// Initializes a new instance of the YamlFrontMatterRoundtripRenderer class.
    /// </summary>
    public YamlFrontMatterRoundtripRenderer()
    {
        _codeBlockRenderer = new CodeBlockRenderer();
    }

    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(RoundtripRenderer renderer, YamlFrontMatterBlock obj)
    {
        renderer.Writer.WriteLine("---");
        _codeBlockRenderer.Write(renderer, obj);
        renderer.Writer.WriteLine("---");
    }
}
