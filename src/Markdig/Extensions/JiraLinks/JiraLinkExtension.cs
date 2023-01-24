// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Normalize.Inlines;
using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.JiraLinks;

/// <summary>
/// Simple inline parser extension for Markdig to find, and 
/// automatically add links to JIRA issue numbers.
/// </summary>
public class JiraLinkExtension : IMarkdownExtension
{
    private readonly JiraLinkOptions _options;

    public JiraLinkExtension(JiraLinkOptions options)
    {
        _options = options;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<JiraLinkInlineParser>())
        {
            // Insert the parser before the link inline parser
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new JiraLinkInlineParser(_options));
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        // No HTML renderer required, since JiraLink type derives from InlineLink (which already has an HTML renderer)

        if (renderer is NormalizeRenderer normalizeRenderer && !normalizeRenderer.ObjectRenderers.Contains<NormalizeJiraLinksRenderer>())
        {
            normalizeRenderer.ObjectRenderers.InsertBefore<LinkInlineRenderer>(new NormalizeJiraLinksRenderer());
        }
    }
}

