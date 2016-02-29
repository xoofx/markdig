using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlCodeBlockRenderer : HtmlRendererBase<CodeBlock>
    {
        protected override void Write(HtmlFormatter visitor, HtmlWriter writer, CodeBlock obj)
        {
            writer.EnsureLine();
            writer.Write("<pre><code");
            var fencedCode = obj as FencedCodeBlock;
            if (fencedCode != null && fencedCode.Language != null)
            {
                writer.Write(" class=\"language-");
                HtmlHelper.EscapeHtml(fencedCode.Language, writer);
                writer.Write("\">");
            }
            WriteLeafRawLines(writer, obj, true, true);
            writer.WriteLine("</code></pre>");
        }
    }
}