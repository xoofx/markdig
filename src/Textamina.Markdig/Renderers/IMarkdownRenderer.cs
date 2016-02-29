using System.Collections.Generic;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers
{
    public interface IMarkdownRenderer
    {
        List<IMarkdownObjectRenderer> ObjectRenderers { get; }

        object Render(MarkdownObject markdownObject);
    }
}