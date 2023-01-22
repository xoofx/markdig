// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Linq;

using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace Markdig.Extensions.ReferralLinks;

public class ReferralLinksExtension : IMarkdownExtension
{
    public ReferralLinksExtension(string[] rels)
    {
        Rels = rels?.ToList() ?? throw new ArgumentNullException(nameof(rels));
    }

    public List<string> Rels { get; }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        string relString = string.Join(" ", Rels.Where(r => !string.IsNullOrEmpty(r)));

        var linkRenderer = renderer.ObjectRenderers.Find<LinkInlineRenderer>();
        if (linkRenderer != null)
        {
            linkRenderer.Rel = relString;
        }

        var autolinkRenderer = renderer.ObjectRenderers.Find<AutolinkInlineRenderer>();
        if (autolinkRenderer != null)
        {
            autolinkRenderer.Rel = relString;
        }
    }
}
