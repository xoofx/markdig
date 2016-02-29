using System.Globalization;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class HeadingRenderer : HtmlObjectRenderer<HeadingBlock>
    {
        protected override void Write(HtmlMarkdownRenderer renderer, HeadingBlock obj)
        {
            var heading = obj.Level.ToString(CultureInfo.InvariantCulture);
            renderer.Write("<h").Write(heading).Write(">");
            renderer.WriteLeafInline(obj);
            renderer.Write("</h").Write(heading).WriteLine(">");
        }
    }
}