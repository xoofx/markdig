// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// Delegates called when processing a document
    /// </summary>
    /// <param name="document">The markdown document.</param>
    public delegate void ProcessDocumentDelegate(MarkdownDocument document);

    /// <summary>
    /// The Markdown parser.
    /// </summary>
    public sealed class MarkdownParser
    {
        private readonly BlockProcessor blockProcessor;
        private readonly InlineProcessor inlineProcessor;
        private readonly MarkdownDocument document;
        private readonly ProcessDocumentDelegate? documentProcessed;
        private readonly bool preciseSourceLocation;
        private readonly bool trackTrivia;

        private readonly int roughLineCountEstimate;

        private LineReader lineReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownParser" /> class.
        /// </summary>
        /// <param name="text">The reader.</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="context">A parser context used for the parsing.</param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        private MarkdownParser(string text, MarkdownPipeline pipeline, MarkdownParserContext? context)
        {
            if (text is null) ThrowHelper.ArgumentNullException_text();
            if (pipeline is null) ThrowHelper.ArgumentNullException(nameof(pipeline));

            trackTrivia = pipeline.TrackTrivia;
            roughLineCountEstimate = text.Length / 40;
            text = FixupZero(text);
            lineReader = new LineReader(text);
            preciseSourceLocation = pipeline.PreciseSourceLocation;

            // Initialize the pipeline
            document = new MarkdownDocument();

            // Initialize the block parsers
            blockProcessor = new BlockProcessor(document, pipeline.BlockParsers, context, pipeline.TrackTrivia);

            // Initialize the inline parsers
            inlineProcessor = new InlineProcessor(document, pipeline.InlineParsers, pipeline.PreciseSourceLocation, context, pipeline.TrackTrivia)
            {
                DebugLog = pipeline.DebugLog
            };

            documentProcessed = pipeline.DocumentProcessed;
        }

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
            pipeline ??= new MarkdownPipelineBuilder().Build();

            // Perform the parsing
            var markdownParser = new MarkdownParser(text, pipeline, context);
            return markdownParser.Parse();
        }

        /// <summary>
        /// Parses the current <see cref="lineReader"/> into a Markdown <see cref="MarkdownDocument"/>.
        /// </summary>
        /// <returns>A document instance</returns>
        private MarkdownDocument Parse()
        {
            if (preciseSourceLocation)
            {
                // Save some List resizing allocations
                document.LineStartIndexes = new List<int>(Math.Min(512, roughLineCountEstimate));
            }

            ProcessBlocks();
            ProcessInlines();

            // At this point the LineIndex is the same as the number of lines in the document
            document.LineCount = blockProcessor.LineIndex;
            
            // Allow to call a hook after processing a document
            documentProcessed?.Invoke(document);
            return document;
        }

        private void ProcessBlocks()
        {
            while (true)
            {
                // Get the precise position of the begining of the line
                var lineText = lineReader.ReadLine();
                
                // If this is the end of file and the last line is empty
                if (lineText.Text is null)
                {
                    if (trackTrivia)
                    {
                        Block? lastBlock = blockProcessor.LastBlock;
                        if (lastBlock is null && document.Count == 0)
                        {
                            // this means we have unassigned characters
                            var noBlocksFoundBlock = new EmptyBlock (null);
                            List<StringSlice> linesBefore = blockProcessor.UseLinesBefore();
                            noBlocksFoundBlock.LinesAfter = new List<StringSlice>();
                            noBlocksFoundBlock.LinesAfter.AddRange(linesBefore);
                            document.Add(noBlocksFoundBlock);
                        }
                        else if (lastBlock != null && blockProcessor.LinesBefore != null)
                        {
                            // this means we're out of lines, but still have unassigned empty lines.
                            // thus, we'll assign the empty unsassigned lines to the last block
                            // of the document.
                            var rootMostContainerBlock = Block.FindRootMostContainerParent(lastBlock);
                            rootMostContainerBlock.LinesAfter ??= new List<StringSlice>();
                            var linesBefore = blockProcessor.UseLinesBefore();
                            rootMostContainerBlock.LinesAfter.AddRange(linesBefore);
                        }
                    }
                    break;
                }
                blockProcessor.ProcessLine(lineText);
            }
            blockProcessor.CloseAll(true);
        }

        /// <summary>
        /// Fixups the zero character by replacing it to a secure character (Section 2.3 Insecure characters, CommonMark specs)
        /// </summary>
        /// <param name="text">The text to secure.</param>
        private string FixupZero(string text)
        {
            return text.Replace('\0', CharHelper.ReplacementChar);
        }

        private void ProcessInlines()
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
                    if (block is LeafBlock leafBlock)
                    {
                        leafBlock.OnProcessInlinesBegin(inlineProcessor);
                        if (leafBlock.ProcessInlines)
                        {
                            inlineProcessor.ProcessInlineLeaf(leafBlock);
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
                    else if (block is ContainerBlock newContainer)
                    {
                        // If we need to remove it
                        if (newContainer.RemoveAfterProcessInlines)
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
                        blocks[blockCount++] = new ContainerItem(newContainer);
                        newContainer.OnProcessInlinesBegin(inlineProcessor);
                        goto process_new_block;
                    }
                }
                container.OnProcessInlinesEnd(inlineProcessor);
                blocks[--blockCount] = default;
            }
        }

        private struct ContainerItem
        {
            public ContainerItem(ContainerBlock container)
            {
                Container = container;
                Index = 0;
            }

            public readonly ContainerBlock Container;

            public int Index;
        }
    }
}