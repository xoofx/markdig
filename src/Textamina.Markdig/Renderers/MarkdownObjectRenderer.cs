using System;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers
{
    public abstract class MarkdownObjectRenderer
    {
        public abstract bool Accept(MarkdownRenderer renderer, Type type);

        public abstract void Write(MarkdownRenderer renderer, MarkdownObject objectToRender);
    }
}