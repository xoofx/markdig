using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers
{
    public class MarkdownParser
    {
        public static TextWriter Log;
        private readonly ParserList<InlineParser> inlineParsers;
        private readonly Document document;
        private readonly BlockParserState blockParserState;
        private readonly StringBuilderCache stringBuilderCache;

        public MarkdownParser(TextReader reader)
        {
            document = new Document();
            Reader = reader;
            stringBuilderCache = new StringBuilderCache();
            blockParserState = new BlockParserState(stringBuilderCache, document);

            inlineParsers = new ParserList<InlineParser>()
            {
                LinkInline.Parser,
                EmphasisInline.Parser,
                EscapeInline.Parser,
                CodeInline.Parser,
                AutolinkInline.Parser,
                HardlineBreakInline.Parser,
                LiteralInline.Parser,
            };
            inlineParsers.Initialize();
        }

        public TextReader Reader { get; }

        public Document Parse()
        {
            ParseLines();
            //ProcessInlines(document);
            return document;
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
                blockParserState.ProcessLine(lineText);
            }
            blockParserState.CloseAll(true);
        }

        private void ProcessInlines(ContainerBlock container)
        {
            var list = new Stack<ContainerBlock>();
            list.Push(container);
            var leafs = new List<Task>();

            while (list.Count > 0)
            {
                container = list.Pop();
                foreach (var block in container.Children)
                {
                    var leafBlock = block as LeafBlock;
                    if (leafBlock != null)
                    {
                        if (leafBlock.ProcessInlines)
                        {
                            var task = new Task(() => ProcessInlineLeaf(leafBlock));
                            task.Start();
                            leafs.Add(task);
                            //ProcessInlineLeaf(leafBlock);
                        }
                    }
                    else 
                    {
                        list.Push((ContainerBlock)block);
                    }
                }
            }

            Task.WaitAll(leafs.ToArray());
        }

        private void ProcessInlineLeaf(LeafBlock leafBlock)
        {
            var lines = leafBlock.Lines;

            leafBlock.Inline = new ContainerInline() {IsClosed = false};
            var inlineState = new InlineParserState(stringBuilderCache, document)
            {
                Text = new StringSlice(leafBlock.Lines.ToString()),
                Inline = leafBlock.Inline,
                Block = leafBlock
            };

            while (!inlineState.Text.IsEndOfSlice)
            {
                var saveLine = inlineState.Text;

                var c = saveLine.CurrentChar;

                var parsers = inlineParsers.GetParsersForOpeningCharacter(c);
                bool match = false;
                if (parsers != null)
                {
                    for (int i = 0; i < parsers.Length; i++)
                    {
                        if (parsers[i].Match(inlineState))
                        {
                            match = true;
                            break;
                        }
                    }
                }
                parsers = inlineParsers.GlobalParsers;
                if (!match && parsers != null)
                {
                    for (int i = 0; i < parsers.Length; i++)
                    {
                        if (parsers[i].Match(inlineState))
                        {
                            break;
                        }
                    }
                }

                var nextInline = inlineState.Inline;

                if (nextInline != null)
                {
                    if (nextInline.Parent == null)
                    {
                        // Get deepest container
                        var container = (ContainerInline)leafBlock.Inline;
                        while (true)
                        {
                            var nextContainer = container.LastChild as ContainerInline;
                            if (nextContainer != null && !nextContainer.IsClosed)
                            {
                                container = nextContainer;
                            }
                            else
                            {
                                break;
                            }
                        }

                        container.AppendChild(nextInline);
                    }

                    if (nextInline.IsClosable && !nextInline.IsClosed)
                    {
                        var inlinesToClose = inlineState.InlinesToClose;
                        var last = inlinesToClose.Count > 0
                            ? inlineState.InlinesToClose[inlinesToClose.Count - 1]
                            : null;
                        if (last != nextInline)
                        {
                            inlineState.InlinesToClose.Add(nextInline);
                        }
                    }
                }
                else
                {
                    // Get deepest container
                    var container = (ContainerInline)leafBlock.Inline;
                    while (true)
                    {
                        var nextContainer = container.LastChild as ContainerInline;
                        if (nextContainer != null && !nextContainer.IsClosed)
                        {
                            container = nextContainer;
                        }
                        else
                        {
                            break;
                        }
                    }

                    inlineState.Inline = container.LastChild is LeafInline ? container.LastChild : container;
                }

                if (Log != null)
                {
                    Log.WriteLine($"** Dump: char '{c}");
                    leafBlock.Inline.DumpTo(Log);
                }
            }

            // Close all inlines not closed
            inlineState.Inline = null;
            foreach (var inline in inlineState.InlinesToClose)
            {
                inline.CloseInternal(inlineState);
            }
            inlineState.InlinesToClose.Clear();

            if (Log != null)
            {
                Log.WriteLine("** Dump before Emphasis:");
                leafBlock.Inline.DumpTo(Log);
                EmphasisInline.ProcessEmphasis(leafBlock.Inline);

                Log.WriteLine();
                Log.WriteLine("** Dump after Emphasis:");
                leafBlock.Inline.DumpTo(Log);
            }
            // TODO: Close opened inlines

            // Close last inline
            //while (inlineStack.Count > 0)
            //{
            //    var inlineState = inlineStack.Pop();
            //    inlineState.Parser.Close(state, inlineState.Inline);
            //}
        }
    }
}