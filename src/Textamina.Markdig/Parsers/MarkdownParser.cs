using System;
using System.Collections.Generic;
using System.IO;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class MarkdownParser
    {
        public static TextWriter Log;
        private readonly BlockParserState blockParserState;
        private readonly InlineParserState inlineState;

        public MarkdownParser(TextReader reader, BlockParserState blockParserState, InlineParserState inlineParserState)
        {
            Reader = reader;
            this.blockParserState = blockParserState;
            this.inlineState = inlineParserState;
        }

        public TextReader Reader { get; }

        public Document Parse()
        {
            ParseLines();
            ProcessInlines(blockParserState.Document);
            return inlineState.Document;
        }

        private void ParseLines()
        {
            while (true)
            {
                var lineText = Reader.ReadLine();

                // If this is the end of file and the last line is empty
                if (lineText == null)
                {
                    break;
                }
                FixupZero(lineText);

                blockParserState.ProcessLine(lineText);
            }
            blockParserState.CloseAll(true);
        }
       
        private unsafe void FixupZero(string text)
        {
            fixed (char* pText = text)
            {
                int length = text.Length;
                for (int i = 0; i < length; i++)
                {
                    var c = pText[i];
                    if (c == '\0')
                    {
                        pText[i] = '\ufffd';
                    }
                }
            }
        }

        private void ProcessInlines(ContainerBlock root)
        {
            var cache = new ObjectCache<ContainerItem>();
            var blocks = new Stack<ContainerItem>();

            blocks.Push(new ContainerItem(root));
            root.OnProcessInlinesBegin(inlineState);
            while (blocks.Count > 0)
            {
                process_new_block:
                var item = blocks.Peek();
                var container = item.Container;

                for (; item.Index < container.Children.Count; item.Index++)
                {
                    var block = container.Children[item.Index];
                    var leafBlock = block as LeafBlock;
                    if (leafBlock != null)
                    {
                        leafBlock.OnProcessInlinesBegin(inlineState);
                        if (leafBlock.ProcessInlines)
                        {
                            inlineState.ProcessInlineLeaf(leafBlock);
                            if (leafBlock.RemoveAfterProcessInlines)
                            {
                                container.Children.RemoveAt(item.Index);
                                item.Index--;
                            }
                            else if (inlineState.BlockNew != null)
                            {
                                container.Children[item.Index] = inlineState.BlockNew;
                            }
                        }
                        leafBlock.OnProcessInlinesEnd(inlineState);
                    }
                    else
                    {
                        var newContainer = (ContainerBlock) block;
                        // If we need to remove it
                        if (newContainer.RemoveAfterProcessInlines)
                        {
                            container.Children.RemoveAt(item.Index);
                        }
                        else
                        {
                            // Else we have processed it
                            item.Index++;
                        }
                        var newItem = cache.Get();
                        newItem.Container = (ContainerBlock)block;
                        block.OnProcessInlinesBegin(inlineState);
                        newItem.Index = 0;
                        blocks.Push(newItem);
                        goto process_new_block;
                    }
                }
                item = blocks.Pop();
                container = item.Container;
                container.OnProcessInlinesEnd(inlineState);

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