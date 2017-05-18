// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.JiraLinks
{
    public class JiraLinkRenderer : HtmlObjectRenderer<JiraLink>
    {
        public JiraLinkOptions Options { get; set; }

        public JiraLinkRenderer(JiraLinkOptions options)
        {
            Options = options;
        }

        protected override void Write(HtmlRenderer renderer, JiraLink obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer
                    .Write("<a href=\"").Write(Options.GetUrl()).Write("/") // <a href="http:/xxx/browse/"
                    .Write(obj.ProjectKey).Write('-').Write(obj.Issue) // XX-1234 (link url)
                    .Write("\"");

                if (Options.OpenInNewWindow)
                {
                    renderer.Write(" target=\"blank\"");
                }

                renderer
                    .WriteAttributes(obj)
                    .Write(">") // >
                    .Write(obj.ProjectKey).Write('-').Write(obj.Issue) // XX-1234 (link text)
                    .Write("</a>"); //</a>
            }
            else
            {
                renderer.Write(obj.ProjectKey).Write('-').Write(obj.Issue); //i.e. just write it out as normal, e.g. XX-1234
            }
        }
    }
}
