// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers
{
    /// <summary>
    /// Base interface for the renderer of a <see cref="MarkdownObject"/>.
    /// </summary>
    public interface IMarkdownObjectRenderer
    {
        /// <summary>
        /// Accepts the specified <see cref="MarkdownObject"/>.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="obj">The Markdown object.</param>
        /// <returns><c>true</c> If this renderer is accepting to render the specified Markdown object</returns>
        bool Accept(RendererBase renderer, MarkdownObject obj);

        /// <summary>
        /// Writes the specified <see cref="MarkdownObject"/> to the <see cref="renderer"/>.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="objectToRender">The object to render.</param>
        void Write(RendererBase renderer, MarkdownObject objectToRender);
    }
}