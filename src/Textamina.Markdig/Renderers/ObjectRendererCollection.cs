// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Renderers
{
    /// <summary>
    /// A collection of <see cref="IMarkdownObjectRenderer"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Helpers.OrderedList{Textamina.Markdig.Renderers.IMarkdownObjectRenderer}" />
    public class ObjectRendererCollection : OrderedList<IMarkdownObjectRenderer>
    {
    }
}