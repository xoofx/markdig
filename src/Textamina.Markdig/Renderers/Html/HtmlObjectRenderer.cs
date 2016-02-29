using System;
using System.Reflection;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public abstract class HtmlObjectRenderer<TObject> : IMarkdownObjectRenderer where TObject : MarkdownObject
    {
        public virtual bool Accept(RendererBase renderer, Type type)
        {
            return typeof(TObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public virtual void Write(RendererBase renderer, MarkdownObject obj)
        {
            Write((HtmlRenderer)renderer, (TObject)obj);
        }

        protected abstract void Write(HtmlRenderer visitor, TObject obj);
    }
}