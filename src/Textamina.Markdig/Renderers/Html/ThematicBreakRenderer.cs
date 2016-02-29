using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class ThematicBreakRenderer : HtmlObjectRenderer<ThematicBreakBlock>
    {
        protected override void Write(HtmlRenderer renderer, ThematicBreakBlock obj)
        {
            renderer.WriteLine("<hr />");
        }
    }
}