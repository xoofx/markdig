// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using Markdig.Syntax.Inlines;

namespace Markdig.Syntax
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
            // TODO: implement a recursiveless method

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

                    // Visit leaf block that have inlines
                    var leafBlock = subBlock as LeafBlock;
                    if (leafBlock?.Inline != null)
                    {
                        foreach (var subInline in Descendants(leafBlock.Inline))
                        {
                            yield return subInline;
                        }
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
        /// Iterates over the descendant elements for the specified markdown <see cref="Inline" /> element and filters by the type {T}.
        /// </summary>
        /// <typeparam name="T">Type to use for filtering the descendants</typeparam>
        /// <param name="inline">The inline markdown object.</param>
        /// <returns>
        /// An iteration over the descendant elements
        /// </returns>
        public static IEnumerable<T> Descendants<T>(this ContainerInline inline) where T : Inline
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

                var subContainer = child as ContainerInline;
                if (subContainer != null)
                {
                    foreach (var sub in subContainer.Descendants<T>())
                    {
                        yield return sub;
                    }
                }

                child = next;
            }
        }

        /// <summary>
        /// Iterates over the descendant elements for the specified markdown <see cref="Block" /> element and filters by the type {T}.
        /// </summary>
        /// <typeparam name="T">Type to use for filtering the descendants</typeparam>
        /// <param name="block">The markdown object.</param>
        /// <returns>
        /// An iteration over the descendant elements
        /// </returns>
        public static IEnumerable<T> Descendants<T>(this ContainerBlock block) where T : Block
        {
            foreach (var subBlock in block)
            {
                var subBlockT = subBlock as T;
                if (subBlockT != null)
                {
                    yield return subBlockT;
                }

                var subBlockContainer = subBlock as ContainerBlock;
                if (subBlockContainer != null)
                {
                    foreach (var sub in subBlockContainer.Descendants<T>())
                    {
                        yield return sub;
                    }
                }
            }
        }
    }
}