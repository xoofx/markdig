using System;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers
{
    public interface IMarkdownObjectRenderer
    {
        bool Accept(RendererBase renderer, Type type);

        void Write(RendererBase renderer, MarkdownObject objectToRender);
    }
}