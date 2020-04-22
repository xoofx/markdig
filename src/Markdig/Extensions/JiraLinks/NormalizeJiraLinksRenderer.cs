using Markdig.Renderers.Normalize;

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
