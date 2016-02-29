using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlBlockRenderer : HtmlRendererBase<HtmlBlock>
    {
        protected override void Write(HtmlFormatter visitor, HtmlWriter writer, HtmlBlock obj)
        {
            WriteLeafRawLines(writer, obj, true, false);
        }
    }
}