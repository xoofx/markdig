// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        private readonly ProcessDocumentDelegate documentProcessed;
        private readonly bool preciseSourceLocation;

        private LineReader lineReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownParser" /> class.
        /// </summary>
        /// <param name="text">The reader.</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        private MarkdownParser(string text, MarkdownPipeline pipeline)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            text = FixupZero(text);
            lineReader = new LineReader(text);
            preciseSourceLocation = pipeline.PreciseSourceLocation;

            // Initialize the pipeline
            var stringBuilderCache = pipeline.StringBuilderCache ?? new StringBuilderCache();

            document = new MarkdownDocument();

            // Initialize the block parsers
            blockProcessor = new BlockProcessor(stringBuilderCache, document, pipeline.BlockParsers);

            // Initialize the inline parsers
            inlineProcessor = new InlineProcessor(stringBuilderCache, document, pipeline.InlineParsers, pipeline.PreciseSourceLocation)
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
        /// <returns>An AST Markdown document</returns>
        /// <exception cref="System.ArgumentNullException">if reader variable is null</exception>
        public static MarkdownDocument Parse(string text, MarkdownPipeline pipeline = null)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            pipeline = pipeline ?? new MarkdownPipelineBuilder().Build();

            // Perform the parsing
            var markdownParser = new MarkdownParser(text, pipeline);
            return markdownParser.Parse();
        }

        /// <summary>
        /// Parses the current <see cref="Reader"/> into a Markdown <see cref="MarkdownDocument"/>.
        /// </summary>
        /// <returns>A document instance</returns>
        private MarkdownDocument Parse()
        {
            ProcessBlocks();
            ProcessInlines();
            
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
                if (lineText == null)
                {
                    break;
                }
                blockProcessor.ProcessLine(lineText.Value);
            }
            blockProcessor.CloseAll(true);
        }

        /// <summary>
        /// Fixups the zero character by replacing it to a secure character (Section 2.3 Insecure characters, CommonMark specs)
        /// </summary>
        /// <param name="text">The text to secure.</param>
        private string FixupZero(string text)
        {
            return text.Replace('\0', CharHelper.ZeroSafeChar);
        }

        private class ContainerItemCache : DefaultObjectCache<ContainerItem>
        {
            protected override void Reset(ContainerItem instance)
            {
                instance.Container = null;
                instance.Index = 0;
            }
        }

        private void ProcessInlines()
        {
            // "stackless" processor
            var cache = new ContainerItemCache();
            var blocks = new Stack<ContainerItem>();

            // TODO: Use an ObjectCache for ContainerItem
            blocks.Push(new ContainerItem(document));
            document.OnProcessInlinesBegin(inlineProcessor);
            while (blocks.Count > 0)
            {
                process_new_block:
                var item = blocks.Peek();
                var container = item.Container;

                for (; item.Index < container.Count; item.Index++)
                {
                    var block = container[item.Index];
                    var leafBlock = block as LeafBlock;
                    if (leafBlock != null)
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
                    else if (block is ContainerBlock)
                    {
                        var newContainer = (ContainerBlock) block;
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
                        var newItem = cache.Get();
                        newItem.Container = (ContainerBlock)block;
                        block.OnProcessInlinesBegin(inlineProcessor);
                        newItem.Index = 0;
                        blocks.Push(newItem);
                        goto process_new_block;
                    }
                }
                item = blocks.Pop();
                container = item.Container;
                container.OnProcessInlinesEnd(inlineProcessor);

                cache.Release(item);
            }
        }

        private class ContainerItem
        {
            public ContainerItem()
            {
            }

            public ContainerItem(ContainerBlock container)
            {
                Container = container;
            }

            public ContainerBlock Container;

            public int Index;
        }
    }
}