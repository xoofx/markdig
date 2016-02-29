using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class CodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            renderer.EnsureLine();
            renderer.Write("<pre><code");
            var fencedCode = obj as FencedCodeBlock;
            if (fencedCode != null && !string.IsNullOrEmpty(fencedCode.Language))
            {
                renderer.Write(" class=\"language-");
                renderer.WriteEscape(fencedCode.Language);
                renderer.Write("\">");
            }
            else
            {
                renderer.Write(">");
            }
            renderer.WriteLeafRawLines(obj, true, true);
            renderer.WriteLine("</code></pre>");
        }
    }
}