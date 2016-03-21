// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// The Markdown parser.
    /// </summary>
    public sealed class MarkdownParser
    {
        private readonly BlockProcessor blockProcessor;
        private readonly InlineProcessor inlineProcessor;
        private readonly MarkdownDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownParser" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        private MarkdownParser(TextReader reader, MarkdownPipeline pipeline)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            Reader = reader;

            // Initialize the pipeline
            pipeline.Initialize();
            var stringBuilderCache = pipeline.StringBuilderCache ?? new StringBuilderCache();

            document = new MarkdownDocument();

            // Initialize the block parsers
            var blockParserList = new BlockParserList();
            blockParserList.AddRange(pipeline.BlockParsers);
            blockProcessor = new BlockProcessor(stringBuilderCache, document, blockParserList);

            // Initialize the inline parsers
            var inlineParserList = new InlineParserList();
            inlineParserList.AddRange(pipeline.InlineParsers);
            inlineProcessor = new InlineProcessor(stringBuilderCache, document, inlineParserList)
            {
                DebugLog = pipeline.DebugLog
            };
        }

        /// <summary>
        /// Parses the specified markdown into an AST <see cref="MarkdownDocument"/>
        /// </summary>
        /// <param name="reader">A Markdown text from a <see cref="TextReader"/>.</param>
        /// <param name="pipeline">The pipeline used for the parsing.</param>
        /// <returns>An AST Markdown document</returns>
        /// <exception cref="System.ArgumentNullException">if reader variable is null</exception>
        public static MarkdownDocument Parse(TextReader reader, MarkdownPipeline pipeline = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            pipeline = pipeline ?? new MarkdownPipeline();

            // Perform the parsing
            var markdownParser = new MarkdownParser(reader, pipeline);
            return markdownParser.Parse();
        }

        /// <summary>
        /// Gets the text reader used.
        /// </summary>
        private TextReader Reader { get; }

        /// <summary>
        /// Parses the current <see cref="Reader"/> into a Markdown <see cref="MarkdownDocument"/>.
        /// </summary>
        /// <returns>A document instance</returns>
        private MarkdownDocument Parse()
        {
            ProcessBlocks();
            ProcessInlines();
            return document;
        }

        private void ProcessBlocks()
        {
            while (true)
            {
                // TODO: A TextReader doesn't allow to precisely track position in file due to line endings
                var lineText = Reader.ReadLine();

                // If this is the end of file and the last line is empty
                if (lineText == null)
                {
                    break;
                }
                lineText = FixupZero(lineText);

                blockProcessor.ProcessLine(new StringSlice(lineText));
            }
            blockProcessor.CloseAll(true);
        }

        /// <summary>
        /// Fixups the zero character by replacing it to a secure character (Section 2.3 Insecure characters, CommonMark specs)
        /// </summary>
        /// <param name="text">The text to secure.</param>
        private string FixupZero(string text)
        {

            int startPos = 0;
            int nextZero;
            StringBuilder newLine = null;
            while ((nextZero = text.IndexOf('\0', startPos)) >= 0)
            {
                if (newLine == null)
                {
                    newLine = StringBuilderCache.Local();
                }
                newLine.Append(text, startPos, nextZero - startPos);
                newLine.Append(CharHelper.ZeroSafeChar);
                startPos = nextZero + 1;
            }

            if (newLine == null)
            {
                return text;
            }
            newLine.Append(text, startPos, text.Length - startPos);

            var result = newLine.ToString();
            newLine.Length = 0;
            return result;
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