// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.AutoLinks
{
    public class NormalizeAutoLinkRenderer : NormalizeObjectRenderer<LinkInline>
    {
        public override bool Accept(RendererBase renderer, MarkdownObject obj)
        {
            if (base.Accept(renderer, obj))
            {
                return renderer is NormalizeRenderer normalizeRenderer
                    && obj is LinkInline link
                    && !normalizeRenderer.Options.ExpandAutoLinks
                    && link.IsAutoLink;
            }
            else
            {
                return false;
            }
        }
        protected override void Write(NormalizeRenderer renderer, LinkInline obj)
        {
            renderer.Write(obj.Url);
        }
    }
}
