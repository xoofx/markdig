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
        /// <para>The descendant elements are returned in DFS-like order.</para>
        /// </summary>
        /// <param name="markdownObject">The markdown object.</param>
        /// <returns>An iteration over the descendant elements</returns>
        public static IEnumerable<MarkdownObject> Descendants(this MarkdownObject markdownObject)
        {
            // Fast-path an object with no children to avoid allocating Stack objects
            if (!(markdownObject is ContainerBlock) && !(markdownObject is ContainerInline)) yield break;

            // TODO: A single Stack<(MarkdownObject block, bool push)> when ValueTuples are available
            Stack<MarkdownObject> stack = new Stack<MarkdownObject>();
            Stack<bool> pushStack = new Stack<bool>();

            stack.Push(markdownObject);
            pushStack.Push(false);

            while (stack.Count > 0)
            {
                var block = stack.Pop();
                var push = pushStack.Pop();
                if (block is ContainerBlock containerBlock)
                {
                    if (push) yield return containerBlock;
                    int subBlockIndex = containerBlock.Count;
                    while (subBlockIndex-- > 0)
                    {
                        var subBlock = containerBlock[subBlockIndex];
                        if (subBlock is LeafBlock leafBlock)
                        {
                            if (leafBlock.Inline != null)
                            {
                                stack.Push(leafBlock.Inline);
                                pushStack.Push(false);
                            }
                        }
                        stack.Push(subBlock);
                        pushStack.Push(true);
                    }
                }
                else if (block is ContainerInline containerInline)
                {
                    if (push) yield return containerInline;
                    var child = containerInline.LastChild;
                    while (child != null)
                    {
                        stack.Push(child);
                        pushStack.Push(true);
                        child = child.PreviousSibling;
                    }
                }
                else yield return block;
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
                T childT = child as T;
                if (childT != null)
                {
                    yield return childT;
                }

                ContainerInline subContainer = child as ContainerInline;
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
                if (subBlock is T subBlockT)
                {
                    yield return subBlockT;
                }

                if (subBlock is ContainerBlock subBlockContainer)
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