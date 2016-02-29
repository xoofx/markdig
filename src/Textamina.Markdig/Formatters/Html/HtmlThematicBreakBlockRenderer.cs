using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlThematicBreakBlockRenderer : HtmlRendererBase<ThematicBreakBlock>
    {
        protected override void Write(HtmlFormatter visitor, HtmlWriter writer, ThematicBreakBlock obj)
        {
            writer.WriteLine("<hr />");
        }
    }
}