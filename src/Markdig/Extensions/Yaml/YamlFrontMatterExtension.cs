// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Renderers;

namespace Markdig.Extensions.Yaml;

/// <summary>
/// Extension to discard a YAML frontmatter at the beginning of a Markdown document.
/// </summary>
public class YamlFrontMatterExtension : IMarkdownExtension
{
    /// <summary>
    /// Allows the <see cref="YamlFrontMatterBlock"/> to appear in the middle of the markdown file.
    /// </summary>
    public bool AllowInMiddleOfDocument { get; set; }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Check the exact type: TomlFrontMatterParser derives from YamlFrontMatterParser,
        // so Contains<YamlFrontMatterParser>() would incorrectly treat TOML as YAML
        // and skip adding the YAML parser (see #980).
        bool hasYamlParser = false;
        for (int i = 0; i < pipeline.BlockParsers.Count; i++)
        {
            if (pipeline.BlockParsers[i].GetType() == typeof(YamlFrontMatterParser))
            {
                hasYamlParser = true;
                break;
            }
        }

        if (!hasYamlParser)
        {
            // Insert the YAML parser before the thematic break parser, as it is also triggered on a --- dash
            pipeline.BlockParsers.InsertBefore<ThematicBreakParser>(new YamlFrontMatterParser { AllowInMiddleOfDocument = AllowInMiddleOfDocument });
        }
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
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
