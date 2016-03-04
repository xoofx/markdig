using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class CodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        public bool OutputAttributesOnPre { get; set; }

        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            renderer.EnsureLine();
            renderer.Write("<pre");
            if (OutputAttributesOnPre)
            {
                renderer.WriteAttributes(obj);
            }
            renderer.Write("><code");
            var fencedCode = obj as FencedCodeBlock;
            if (fencedCode != null && !string.IsNullOrEmpty(fencedCode.Language))
            {
                // TODO: this could be output on the pre attribute instead
                // If the fenced code has already some attributes, try to use them before adding the class language
                var attributes = obj.TryGetAttributes();
                if (OutputAttributesOnPre || attributes == null)
                {
                    renderer.Write(" class=\"language-");
                    renderer.WriteEscape(fencedCode.Language);
                    renderer.Write("\"");
                }
                else
                {
                    attributes.AddClass("language-" + fencedCode.Language);
                    renderer.WriteAttributes(obj);
                }
            }
            renderer.Write(">");
            renderer.WriteLeafRawLines(obj, true, true);
            renderer.WriteLine("</code></pre>");
        }
    }
}