// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Diagrams
{
    /// <summary>
    /// Extension to allow diagrams.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class DiagramExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                var codeRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
                // TODO: Add other well known diagram languages
                codeRenderer.BlocksAsDiv.Add("mermaid");
            }
        }
    }
}