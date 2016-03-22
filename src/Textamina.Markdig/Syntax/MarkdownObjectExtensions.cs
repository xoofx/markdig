// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Extensions for visiting <see cref="Block"/> or <see cref="Inline"/>
    /// </summary>
    public static class MarkdownObjectExtensions
    {
        /// <summary>
        /// Iterates over the descendant elements for the specified markdown element, including <see cref="Block"/> and <see cref="Inline"/>.
        /// </summary>
        /// <param name="markdownObject">The markdown object.</param>
        /// <returns>An iteration over the descendant elements</returns>
        public static IEnumerable<MarkdownObject> Descendants(this MarkdownObject markdownObject)
        {
            var block = markdownObject as ContainerBlock;
            if (block != null)
            {
                foreach (var subBlock in block)
                {
                    yield return subBlock;

                    foreach (var sub in subBlock.Descendants())
                    {
                        yield return sub;
                    }
                }
            }
            else
            {
                var inline = markdownObject as ContainerInline;
                if (inline != null)
                {
                    var child = inline.FirstChild;
                    while (child != null)
                    {
                        var next = child.NextSibling;
                        yield return child;

                        foreach (var sub in child.Descendants())
                        {
                            yield return sub;
                        }

                        child = next;
                    }
                }
            }
        }

        /// <summary>
        /// Iterates over the descendant elements for the specified markdown element and filters by the type {T}, including <see cref="Block" /> and <see cref="Inline" />.
        /// </summary>
        /// <typeparam name="T">Type to use for filtering the descendants</typeparam>
        /// <param name="markdownObject">The markdown object.</param>
        /// <returns>
        /// An iteration over the descendant elements
        /// </returns>
        public static IEnumerable<T> Descendants<T>(this MarkdownObject markdownObject) where T : MarkdownObject
        {
            var block = markdownObject as ContainerBlock;
            if (block != null)
            {
                foreach (var subBlock in block)
                {
                    var subBlockT = subBlock as T;
                    if (subBlockT != null)
                    {
                        yield return subBlockT;
                    }

                    foreach (var sub in subBlock.Descendants<T>())
                    {
                        yield return sub;
                    }
                }
            }
            else // if (typeof(Inline).IsAssignableFrom(typeof(T)))  // TODO: Optimize for the case where T is not an inline
            {
                var inline = markdownObject as ContainerInline;
                if (inline != null)
                {
                    var child = inline.FirstChild;
                    while (child != null)
                    {
                        var next = child.NextSibling;
                        var childT = child as T;
                        if (childT != null)
                        {
                            yield return childT;
                        }

                        foreach (var sub in child.Descendants<T>())
                        {
                            yield return sub;
                        }

                        child = next;
                    }
                }
            }
        }
    }
}