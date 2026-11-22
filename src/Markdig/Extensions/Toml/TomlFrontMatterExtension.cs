// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Yaml;
using Markdig.Renderers;

namespace Markdig.Extensions.Toml;

/// <summary>
/// Extension to discard a TOML frontmatter at the beginning of a Markdown document.
/// </summary>
public sealed class TomlFrontMatterExtension : IMarkdownExtension
{
    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.BlockParsers.Contains<TomlFrontMatterParser>())
        {
            // A TOML front matter can only start the document: insert its parser first, before the list parser
            // that also opens on `+`, whichever other parsers the pipeline keeps.
            pipeline.BlockParsers.Insert(0, new TomlFrontMatterParser());
        }
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        // TomlFrontMatterBlock derives from YamlFrontMatterBlock, so the YAML renderers are reused.
        if (!renderer.ObjectRenderers.Contains<YamlFrontMatterHtmlRenderer>())
        {
            renderer.ObjectRenderers.InsertBefore<Renderers.Html.CodeBlockRenderer>(new YamlFrontMatterHtmlRenderer());
        }

        if (!renderer.ObjectRenderers.Contains<YamlFrontMatterRoundtripRenderer>())
        {
            renderer.ObjectRenderers.InsertBefore<Renderers.Roundtrip.CodeBlockRenderer>(new YamlFrontMatterRoundtripRenderer());
        }
    }
}