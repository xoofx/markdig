// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Runtime.CompilerServices;

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers;

/// <summary>
/// Delegates called when processing a document
/// </summary>
/// <param name="document">The markdown document.</param>
public delegate void ProcessDocumentDelegate(MarkdownDocument document);

/// <summary>
/// The Markdown parser.
/// </summary>
public static class MarkdownParser
{
    /// <summary>
    /// Parses the specified markdown into an AST <see cref="MarkdownDocument"/>
    /// </summary>
    /// <param name="text">A Markdown text</param>
    /// <param name="pipeline">The pipeline used for the parsing.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <returns>An AST Markdown document</returns>
    /// <exception cref="ArgumentNullException">if reader variable is null</exception>
    public static MarkdownDocument Parse(string text, MarkdownPipeline? pipeline = null, MarkdownParserContext? context = null)
    {
        if (text is null) ThrowHelper.ArgumentNullException_text();

        pipeline ??= Markdown.DefaultPipeline;

        text = FixupZero(text);

        var document = new MarkdownDocument
        {
            IsOpen = true
        };

        if (pipeline.PreciseSourceLocation)
        {
            int roughLineCountEstimate = text.Length / 32;
            roughLineCountEstimate = Math.Max(4, Math.Min(512, roughLineCountEstimate));
            document.LineStartIndexes = new List<int>(roughLineCountEstimate);
        }

        var blockProcessor = BlockProcessor.Rent(document, pipeline.BlockParsers, context, pipeline.TrackTrivia);
        try
        {
            blockProcessor.Open(document);

            ProcessBlocks(blockProcessor, text);

            if (pipeline.TrackTrivia)
            {
                ProcessBlocksTrivia(blockProcessor, document);
            }

            // At this point the LineIndex is the same as the number of lines in the document
            document.LineCount = blockProcessor.LineIndex;
        }
        finally
        {
            BlockProcessor.Release(blockProcessor);
        }

        var inlineProcessor = InlineProcessor.Rent(document, pipeline.InlineParsers, pipeline.PreciseSourceLocation, context, pipeline.TrackTrivia);
        inlineProcessor.DebugLog = pipeline.DebugLog;
        try
        {
            ProcessInlines(inlineProcessor, document);
        }
        finally
        {
            InlineProcessor.Release(inlineProcessor);
        }

        // Allow to call a hook after processing a document
        pipeline.DocumentProcessed?.Invoke(document);

        return document;
    }

    /// <summary>
    /// Fixups the zero character by replacing it to a secure character (Section 2.3 Insecure characters, CommonMark specs)
    /// </summary>
    /// <param name="text">The text to secure.</param>
    private static string FixupZero(string text)
    {
        return text.Replace('\0', CharHelper.ReplacementChar);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProcessBlocks(BlockProcessor blockProcessor, string text)
    {
        var lineReader = new LineReader(text);

        while (true)
        {
            // Get the precise position of the beginning of the line
            StringSlice lineText = lineReader.ReadLine();

            // If this is the end of file and the last line is empty
            if (lineText.Text is null)
            {
                break;
            }

            blockProcessor.ProcessLine(lineText);
        }

        blockProcessor.CloseAll(true);
    }

    private static void ProcessBlocksTrivia(BlockProcessor blockProcessor, MarkdownDocument document)
    {
        Block? lastBlock = blockProcessor.LastBlock;
        if (lastBlock is null && document.Count == 0)
        {
            // this means we have unassigned characters
            var noBlocksFoundBlock = new EmptyBlock(null);
            List<StringSlice> linesBefore = blockProcessor.UseLinesBefore();
            noBlocksFoundBlock.LinesAfter = [];
            if (linesBefore != null)
            {
                noBlocksFoundBlock.LinesAfter.AddRange(linesBefore);
            }

            document.Add(noBlocksFoundBlock);
        }
        else if (lastBlock != null && blockProcessor.LinesBefore != null)
        {
            // this means we're out of lines, but still have unassigned empty lines.
            // thus, we'll assign the empty unsassigned lines to the last block
            // of the document.
            var rootMostContainerBlock = Block.FindRootMostContainerParent(lastBlock);
            rootMostContainerBlock.LinesAfter ??= [];
            var linesBefore = blockProcessor.UseLinesBefore();
            rootMostContainerBlock.LinesAfter.AddRange(linesBefore);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProcessInlines(InlineProcessor inlineProcessor, MarkdownDocument document)
    {
        // "stackless" processor
        int blockCount = 1;
        var blocks = new ContainerItem[4];

        blocks[0] = new ContainerItem(document);
        document.OnProcessInlinesBegin(inlineProcessor);

        while (blockCount != 0)
        {
            process_new_block:
            ref ContainerItem item = ref blocks[blockCount - 1];
            var container = item.Container;

            for (; item.Index < container.Count; item.Index++)
            {
                var block = container[item.Index];
                if (block.IsLeafBlock)
                {
                    LeafBlock leafBlock = Unsafe.As<LeafBlock>(block);
                    leafBlock.OnProcessInlinesBegin(inlineProcessor);
                    if (leafBlock.ProcessInlines)
                    {
                        inlineProcessor.ProcessInlineLeaf(leafBlock);

                        // Experimental code to handle a replacement of a parent container
                        // Not satisfied with this code, so we are keeping it internal for now
                        if (inlineProcessor.PreviousContainerToReplace != null)
                        {
                            if (container == inlineProcessor.PreviousContainerToReplace)
                            {
                                item = new ContainerItem(inlineProcessor.NewContainerToReplace!) { Index = item.Index };
                                container = item.Container;
                            }
                            else
                            {
                                bool parentBlockFound = false;
                                for (int i = blockCount - 2; i >= 0; i--)
                                {
                                    ref var parentBlock = ref blocks[i];
                                    if (parentBlock.Container == inlineProcessor.PreviousContainerToReplace)
                                    {
                                        parentBlock = new ContainerItem(inlineProcessor.NewContainerToReplace!) { Index = parentBlock.Index };
                                        parentBlockFound = true;
                                        break;
                                    }
                                }

                                if (!parentBlockFound)
                                {
                                    throw new InvalidOperationException("Cannot find the parent block to replace");
                                }
                            }

                            inlineProcessor.PreviousContainerToReplace = null;
                            inlineProcessor.NewContainerToReplace = null;
                        }

                        if (leafBlock.RemoveAfterProcessInlines)
                        {
                            container.RemoveAt(item.Index);
                            item.Index--;
                        }
                        else if (inlineProcessor.BlockNew != null)
                        {
                            container[item.Index] = inlineProcessor.BlockNew;
                        }
                    }
                    leafBlock.OnProcessInlinesEnd(inlineProcessor);
                }
                else if (block.IsContainerBlock)
                {
                    // If we need to remove it
                    if (block.RemoveAfterProcessInlines)
                    {
                        container.RemoveAt(item.Index);
                    }
                    else
                    {
                        // Else we have processed it
                        item.Index++;
                    }

                    if (blockCount == blocks.Length)
                    {
                        Array.Resize(ref blocks, blockCount * 2);
                        ThrowHelper.CheckDepthLimit(blocks.Length);
                    }
                    blocks[blockCount++] = new ContainerItem(Unsafe.As<ContainerBlock>(block));
                    block.OnProcessInlinesBegin(inlineProcessor);
                    goto process_new_block;
                }
            }
            container.OnProcessInlinesEnd(inlineProcessor);
            blocks[--blockCount] = default;
        }
    }

    private struct ContainerItem(ContainerBlock container)
    {
        public readonly ContainerBlock Container = container;

        public int Index = 0;
    }
}