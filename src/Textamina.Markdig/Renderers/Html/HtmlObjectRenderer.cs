// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Reflection;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public abstract class HtmlObjectRenderer<TObject> : IMarkdownObjectRenderer where TObject : MarkdownObject
    {
        public virtual bool Accept(RendererBase renderer, object obj)
        {
            return obj is TObject;
        }

        public virtual void Write(RendererBase renderer, MarkdownObject obj)
        {
            Write((HtmlRenderer)renderer, (TObject)obj);
        }

        protected abstract void Write(HtmlRenderer visitor, TObject obj);
    }
}