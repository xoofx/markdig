using System.Globalization;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlHeadingBlockRenderer : HtmlRendererBase<HeadingBlock>
    {
        protected override void Write(HtmlFormatter formatter, HtmlWriter writer, HeadingBlock obj)
        {
            var heading = obj.Level.ToString(CultureInfo.InvariantCulture);
            writer.Write("<h").Write(heading).Write(">");
            WriteLeafInline(formatter, writer, obj);
            writer.Write("</h").Write(heading).Write(">");
        }
    }
}