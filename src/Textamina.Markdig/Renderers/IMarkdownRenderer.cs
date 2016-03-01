using System.Collections.Generic;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers
{
    public interface IMarkdownRenderer
    {
        ObjectRendererCollection ObjectRenderers { get; }

        object Render(MarkdownObject markdownObject);
    }
}