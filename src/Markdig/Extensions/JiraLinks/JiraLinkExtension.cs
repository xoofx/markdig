using System;
using System.Collections.Generic;
using System.Text;
using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;

namespace Markdig.Extensions.JiraLinks
{
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
                pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new JiraLinkInlineParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<JiraLinkRenderer>())
            {
                htmlRenderer.ObjectRenderers.Add(new JiraLinkRenderer(_options));
            }
        }
    }
    
}
