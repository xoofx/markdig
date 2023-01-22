// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics;

using Markdig.Syntax.Inlines;

namespace Markdig.Syntax;

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
        var stack = new Stack<MarkdownObject>();
        var pushStack = new Stack<bool>();

        stack.Push(markdownObject);
        pushStack.Push(false);

        while (stack.Count > 0)
        {
            var block = stack.Pop();
            if (pushStack.Pop()) yield return block;
            if (block is ContainerBlock containerBlock)
            {
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
                var child = containerInline.LastChild;
                while (child != null)
                {
                    stack.Push(child);
                    pushStack.Push(true);
                    child = child.PreviousSibling;
                }
            }
        }
    }

    /// <summary>
    /// Iterates over the descendant elements for the specified markdown element, including <see cref="Block"/> and <see cref="Inline"/> and filters by the type <typeparamref name="T"/>.
    /// <para>The descendant elements are returned in DFS-like order.</para>
    /// </summary>
    /// <typeparam name="T">Type to use for filtering the descendants</typeparam>
    /// <param name="markdownObject">The markdown object.</param>
    /// <returns>An iteration over the descendant elements</returns>
    public static IEnumerable<T> Descendants<T>(this MarkdownObject markdownObject) where T : MarkdownObject
    {
        if (typeof(T).IsSubclassOf(typeof(Block)))
        {
            if (markdownObject is ContainerBlock containerBlock && containerBlock.Count > 0)
            {
                return BlockDescendantsInternal<T>(containerBlock);
            }
        }
        else // typeof(T).IsSubclassOf(typeof(Inline)))
        {
            if (markdownObject is ContainerBlock containerBlock)
            {
                if (containerBlock.Count > 0)
                {
                    return InlineDescendantsInternal<T>(containerBlock);
                }
            }
            else if (markdownObject is ContainerInline containerInline && containerInline.FirstChild != null)
            {
                return containerInline.FindDescendantsInternal<T>();
            }
        }

        return Array.Empty<T>();
    }

    /// <summary>
    /// Iterates over the descendant elements for the specified markdown <see cref="Inline" /> element and filters by the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type to use for filtering the descendants</typeparam>
    /// <param name="inline">The inline markdown object.</param>
    /// <returns>
    /// An iteration over the descendant elements
    /// </returns>
    public static IEnumerable<T> Descendants<T>(this ContainerInline inline) where T : Inline
        => inline.FindDescendants<T>();

    /// <summary>
    /// Iterates over the descendant elements for the specified markdown <see cref="Block" /> element and filters by the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type to use for filtering the descendants</typeparam>
    /// <param name="block">The markdown object.</param>
    /// <returns>
    /// An iteration over the descendant elements
    /// </returns>
    public static IEnumerable<T> Descendants<T>(this ContainerBlock block) where T : Block
    {
        if (block is { Count: > 0 })
        {
            return BlockDescendantsInternal<T>(block);
        }
        else
        {
            return Array.Empty<T>();
        }
    }

    private static IEnumerable<T> BlockDescendantsInternal<T>(ContainerBlock block) where T : MarkdownObject
    {
        Debug.Assert(typeof(T).IsSubclassOf(typeof(Block)));

        var stack = new Stack<Block>();

        int childrenCount = block.Count;
        while (childrenCount-- > 0)
        {
            stack.Push(block[childrenCount]);
        }

        while (stack.Count > 0)
        {
            var subBlock = stack.Pop();
            if (subBlock is T subBlockT)
            {
                yield return subBlockT;
            }

            if (subBlock is ContainerBlock subBlockContainer)
            {
                childrenCount = subBlockContainer.Count;
                while (childrenCount-- > 0)
                {
                    stack.Push(subBlockContainer[childrenCount]);
                }
            }
        }
    }

    private static IEnumerable<T> InlineDescendantsInternal<T>(ContainerBlock block) where T : MarkdownObject
    {
        Debug.Assert(typeof(T).IsSubclassOf(typeof(Inline)));

        foreach (MarkdownObject descendant in block.Descendants())
        {
            if (descendant is T descendantT)
            {
                yield return descendantT;
            }
        }
    }
}