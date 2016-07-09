// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace Markdig.Extensions.NoRefLinks
{
    public class NoRefLinksExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(IMarkdownRenderer renderer)
        {
            var linkRenderer = renderer.ObjectRenderers.Find<LinkInlineRenderer>();
            if (linkRenderer != null)
            {
                linkRenderer.AutoRelNoFollow = true;
            }

            var autolinkRenderer = renderer.ObjectRenderers.Find<AutolinkInlineRenderer>();
            if (autolinkRenderer != null)
            {
                autolinkRenderer.AutoRelNoFollow = true;
            }
        }
    }
}
