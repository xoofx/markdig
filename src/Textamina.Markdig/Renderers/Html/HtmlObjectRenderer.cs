// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html
{
    /// <summary>
    /// A base class for HTML rendering <see cref="Block"/> and <see cref="Inline"/> Markdown objects.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <seealso cref="Textamina.Markdig.Renderers.IMarkdownObjectRenderer" />
    public abstract class HtmlObjectRenderer<TObject> : IMarkdownObjectRenderer where TObject : MarkdownObject
    {
        public virtual bool Accept(RendererBase renderer, MarkdownObject obj)
        {
            return obj is TObject;
        }

        public virtual void Write(RendererBase renderer, MarkdownObject obj)
        {
            Write((HtmlRenderer)renderer, (TObject)obj);
        }

        /// <summary>
        /// Writes the specified Markdown object to the renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="obj">The markdown object.</param>
        protected abstract void Write(HtmlRenderer renderer, TObject obj);
    }
}