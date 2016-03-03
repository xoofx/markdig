using System.Collections.Generic;
using System.IO;
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

        private void ProcessInlines(ContainerBlock container)
        {
            var list = new Stack<ContainerBlock>();
            list.Push(container);
            while (list.Count > 0)
            {
                container = list.Pop();
                for (int i = 0; i < container.Children.Count; i++)
                {
                    var block = container.Children[i];
                    var leafBlock = block as LeafBlock;
                    if (leafBlock != null)
                    {
                        if (leafBlock.ProcessInlines)
                        {
                            inlineState.ProcessInlineLeaf(leafBlock);
                            if (leafBlock.RemoveAfterProcessInlines)
                            {
                                container.Children.RemoveAt(i);
                                i--;
                            }
                            else if (inlineState.BlockNew != null)
                            {
                                container.Children[i] = inlineState.BlockNew;
                            }
                        }
                    }
                    else
                    {
                        list.Push((ContainerBlock) block);
                    }
                }
                if (container.RemoveAfterProcessInlines)
                {
                    container.Parent.Children.Remove(container);
                }
            }
        }
    }
}