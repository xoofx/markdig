// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.NonAsciiNoEscape
{
    /// <summary>
    /// Extension that will disable URI escape with % characters for non-US-ASCII characters in order to workaround a bug under IE/Edge with local file links containing non US-ASCII chars. DO NOT USE OTHERWISE.
    /// </summary>
    public class NonAsciiNoEscapeExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.UseNonAsciiNoEscape = true;
            }
        }
    }
}