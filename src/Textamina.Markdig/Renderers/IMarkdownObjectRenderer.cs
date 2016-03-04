// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers
{
    public interface IMarkdownObjectRenderer
    {
        bool Accept(RendererBase renderer, object obj);

        void Write(RendererBase renderer, MarkdownObject objectToRender);
    }
}