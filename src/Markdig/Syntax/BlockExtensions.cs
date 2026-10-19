// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Runtime.CompilerServices;

using Markdig.Extensions.Footnotes;

namespace Markdig.Syntax;

/// <summary>
/// Extensions for <see cref="Block"/>
/// </summary>
public static class BlockExtensions
{
    // TODO: Add test for this code

    /// <summary>
    /// Performs the find block at position operation.
    /// </summary>
    public static Block? FindBlockAtPosition(this Block rootBlock, int position)
    {
        var contains = rootBlock.CompareToPosition(position) == 0;
        if (!(rootBlock is ContainerBlock blocks) || blocks.Count == 0 || !contains)
        {
            return contains ? rootBlock : null;
        }

        var skipGroups = blocks is MarkdownDocument;
        var lowerIndex = 0;
        var upperIndex = blocks.Count - 1;

        // binary search on lines
        Block? block = null;
        while (lowerIndex <= upperIndex)
        {
            int midIndex = (upperIndex - lowerIndex) / 2 + lowerIndex;
            block = blocks[midIndex];
            if (skipGroups && IsDefinitionGroup(block))
            {
                midIndex = SkipDefinitionGroups(blocks, midIndex, lowerIndex, upperIndex);
                if (midIndex < 0)
                {
                    block = null;
                    break;
                }
                block = blocks[midIndex];
            }

            int comparison = block.CompareToPosition(position);
            if (comparison == 0)
            {
                break;
            }

            block = null;
            if (comparison < 0)
                lowerIndex = midIndex + 1;
            else
                upperIndex = midIndex - 1;
        }

        if (block is null)
        {
            // The position might be within a definition collected by a group (e.g. a link reference definition or a footnote)
            block = skipGroups ? FindDefinitionAtPosition((MarkdownDocument)blocks, position) : null;
            if (block is null)
            {
                return rootBlock;
            }
        }

        // Recursively go deep into the block
        return FindBlockAtPosition(block, position);
    }

    /// <summary>
    /// Performs the find closest line operation.
    /// </summary>
    public static int FindClosestLine(this MarkdownDocument root, int line)
    {
        var closestBlock = root.FindClosestBlock(line);
        return closestBlock?.Line ?? 0;
    }

    /// <summary>
    /// Performs the find closest block operation.
    /// </summary>
    public static Block? FindClosestBlock(this Block rootBlock, int line)
    {
        if (!(rootBlock is ContainerBlock blocks) || blocks.Count == 0)
        {
            return rootBlock.Line == line ? rootBlock : null;
        }

        if (blocks is not MarkdownDocument document)
        {
            return FindClosestChild(blocks, line, skipGroups: false);
        }

        var closestBlock = FindClosestChild(document, line, skipGroups: true);
        if (closestBlock?.Line != line)
        {
            // A definition collected by a group (e.g. a link reference definition or a footnote) is only selected on an exact line match.
            // If the document contains only groups, search them as regular children.
            closestBlock = FindDefinitionAtLine(document, line) ?? closestBlock ?? FindClosestChild(document, line, skipGroups: false);
        }

        return closestBlock;
    }

    private static Block? FindClosestChild(ContainerBlock blocks, int line, bool skipGroups)
    {
        var lowerIndex = 0;
        var upperIndex = blocks.Count - 1;

        // binary search on lines
        while (lowerIndex <= upperIndex)
        {
            int midIndex = (upperIndex - lowerIndex) / 2 + lowerIndex;
            var block = blocks[midIndex];
            if (skipGroups && IsDefinitionGroup(block))
            {
                midIndex = SkipDefinitionGroups(blocks, midIndex, lowerIndex, upperIndex);
                if (midIndex < 0)
                {
                    break;
                }
                block = blocks[midIndex];
            }

            int comparison = block.Line.CompareTo(line);
            if (comparison == 0)
            {
                return block;
            }
            if (comparison < 0)
                lowerIndex = midIndex + 1;
            else
                upperIndex = midIndex - 1;
        }

        // The blocks before and after the line (when skipping groups, they are never groups as they have been compared)
        var prevIndex = lowerIndex - 1;
        var nextIndex = upperIndex + 1;

        // If we are between two lines, try to find the best spot
        if (prevIndex >= 0 && nextIndex < blocks.Count)
        {
            var prevBlock = blocks[prevIndex].FindClosestBlock(line) ?? blocks[prevIndex];
            var nextBlock = blocks[nextIndex].FindClosestBlock(line) ?? blocks[nextIndex];

            if (prevBlock.Line == line)
            {
                return prevBlock;
            }

            if (nextBlock.Line == line)
            {
                return nextBlock;
            }

            // we calculate the position of the current line relative to the line found and previous line
            var prevLine = prevBlock.Line;
            var nextLine = nextBlock.Line;

            var middle = (line - prevLine) * 1.0 / (nextLine - prevLine);
            // If  relative position < 0.5, we select the previous line, otherwise we select the line found
            return middle < 0.5 ? prevBlock : nextBlock;
        }

        if (nextIndex < blocks.Count)
        {
            var prevBlock = blocks[nextIndex].FindClosestBlock(line) ?? blocks[nextIndex];
            return prevBlock;
        }

        if (prevIndex >= 0)
        {
            var prevBlock = blocks[prevIndex].FindClosestBlock(line) ?? blocks[prevIndex];
            return prevBlock;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDefinitionGroup(Block block) => block.IsContainerBlock && (block is LinkReferenceDefinitionGroup or FootnoteGroup);

    /// <summary>
    /// Returns the index of the block nearest to the group at <paramref name="midIndex"/> within [<paramref name="lowerIndex"/>, <paramref name="upperIndex"/>]
    /// that is not a <see cref="LinkReferenceDefinitionGroup"/> or a <see cref="FootnoteGroup"/>, or -1 if there are only groups in this range.
    /// </summary>
    /// <remarks>
    /// These groups are created by the parsers to collect definitions and are only added to the document.
    /// They are not located in the source (their line is 0 and their span starts at 0) and they can be anywhere
    /// among the children of the document (e.g. a footnote group is moved to the end of the document),
    /// so they must not be compared during a binary search. A document has at most one group of each kind.
    /// </remarks>
    private static int SkipDefinitionGroups(ContainerBlock blocks, int midIndex, int lowerIndex, int upperIndex)
    {
        for (int i = midIndex + 1; i <= upperIndex; i++)
        {
            if (!IsDefinitionGroup(blocks[i]))
            {
                return i;
            }
        }

        for (int i = midIndex - 1; i >= lowerIndex; i--)
        {
            if (!IsDefinitionGroup(blocks[i]))
            {
                return i;
            }
        }

        return -1;
    }

    private static Block? FindDefinitionAtLine(MarkdownDocument document, int line)
    {
        return FindGroupChildAtLine(document, document.GetLinkReferenceDefinitionGroupOrNull(), line)
            ?? FindGroupChildAtLine(document, GetFootnoteGroup(document), line);
    }

    private static Block? FindDefinitionAtPosition(MarkdownDocument document, int position)
    {
        return FindGroupChildAtPosition(document, document.GetLinkReferenceDefinitionGroupOrNull(), position)
            ?? FindGroupChildAtPosition(document, GetFootnoteGroup(document), position);
    }

    // The footnote group is moved to the end of the document once the inlines have been processed
    private static FootnoteGroup? GetFootnoteGroup(MarkdownDocument document) => document.LastChild as FootnoteGroup;

    private static Block? FindGroupChildAtLine(MarkdownDocument document, ContainerBlock? group, int line)
    {
        // The group might not be attached to the document (e.g. link reference definitions when tracking trivia)
        if (group?.Parent == document)
        {
            foreach (var child in group)
            {
                var block = child.FindClosestBlock(line) ?? child;
                if (block.Line == line)
                {
                    return block;
                }
            }
        }

        return null;
    }

    private static Block? FindGroupChildAtPosition(MarkdownDocument document, ContainerBlock? group, int position)
    {
        if (group?.Parent == document)
        {
            foreach (var child in group)
            {
                if (child.CompareToPosition(position) == 0)
                {
                    return child;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Performs the contains position operation.
    /// </summary>
    public static bool ContainsPosition(this Block block, int position)
    {
        return CompareToPosition(block, position) == 0;
    }

    /// <summary>
    /// Performs the compare to position operation.
    /// </summary>
    public static int CompareToPosition(this Block block, int position)
    {
        return position < block.Span.Start ? 1 : position > block.Span.End + 1 ? -1 : 0;
    }
}
