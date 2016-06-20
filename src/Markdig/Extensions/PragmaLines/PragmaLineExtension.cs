// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Syntax;

namespace Markdig.Extensions.PragmaLines
{
    /// <summary>
    /// Extension to a span for each line containing the original line id (using id = pragma-line#line_number_zero_based)
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class PragmaLineExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectWriteBefore -= HtmlRendererOnObjectWriteBefore;
                htmlRenderer.ObjectWriteBefore += HtmlRendererOnObjectWriteBefore;
            }
        }

        private static void HtmlRendererOnObjectWriteBefore(IMarkdownRenderer renderer, MarkdownObject markdownObject)
        {
            if (markdownObject is Block)
            {
                var htmlRenderer = (HtmlRenderer) renderer;
                htmlRenderer.EnsureLine();
                htmlRenderer.WriteLine($"<span id=\"pragma-line-{markdownObject.Line}\"></span>");
            }
        }
    }
}