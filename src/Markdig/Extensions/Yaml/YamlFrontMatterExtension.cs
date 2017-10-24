// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Yaml
{
    /// <summary>
    /// Extension to discard a YAML frontmatter at the beginning of a Markdown document.
    /// </summary>
    public class YamlFrontMatterExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<YamlFrontMatterParser>())
            {
                // Insert the YAML parser before the thematic break parser, as it is also triggered on a --- dash
                pipeline.BlockParsers.InsertBefore<ThematicBreakParser>(new YamlFrontMatterParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (!renderer.ObjectRenderers.Contains<YamlFrontMatterRenderer>())
            {
                renderer.ObjectRenderers.InsertBefore<CodeBlockRenderer>(new YamlFrontMatterRenderer());
            }
        }
    }
}