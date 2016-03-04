// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers
{
    /// <summary>
    /// Base interface for a renderer for a Markdown <see cref="Document"/>.
    /// </summary>
    public interface IMarkdownRenderer
    {
        /// <summary>
        /// Gets the object renderers that will render <see cref="Block"/> and <see cref="Inline"/> elements.
        /// </summary>
        ObjectRendererCollection ObjectRenderers { get; }

        /// <summary>
        /// Renders the specified markdown object.
        /// </summary>
        /// <param name="markdownObject">The markdown object.</param>
        /// <returns>The result of the rendering.</returns>
        object Render(MarkdownObject markdownObject);
    }
}