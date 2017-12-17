using Markdig.Renderers.Normalize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Markdig.Extensions.JiraLinks
{
    public class NormalizeJiraLinksRenderer : NormalizeObjectRenderer<JiraLink>
    {
        protected override void Write(NormalizeRenderer renderer, JiraLink obj)
        {
            renderer.Write(obj.ProjectKey);
            renderer.Write("-");
            renderer.Write(obj.Issue);
        }
    }
}
