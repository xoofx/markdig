using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class HtmlBlockRenderer : HtmlObjectRenderer<HtmlBlock>
    {
        protected override void Write(HtmlRenderer renderer, HtmlBlock obj)
        {
            renderer.WriteLeafRawLines(obj, true, false);
        }
    }
}