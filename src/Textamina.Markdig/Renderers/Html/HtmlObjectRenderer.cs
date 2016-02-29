using System;
using System.Reflection;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public abstract class HtmlObjectRenderer<TObject> : MarkdownObjectRenderer where TObject : MarkdownObject
    {
        public override bool Accept(MarkdownRenderer renderer, Type type)
        {
            return typeof(TObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public override void Write(MarkdownRenderer renderer, MarkdownObject obj)
        {
            Write((HtmlMarkdownRenderer)renderer, (TObject)obj);
        }

        protected abstract void Write(HtmlMarkdownRenderer visitor, TObject obj);
    }
}