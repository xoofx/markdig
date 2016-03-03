using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers
{
    public interface IMarkdownRenderer
    {
        ObjectRendererCollection OpeningObjectRenderers { get; }

        ObjectRendererCollection ObjectRenderers { get; }

        ObjectRendererCollection ClosingObjectRenderers { get; }

        object Render(MarkdownObject markdownObject);
    }
}