using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers
{
    public class MarkdownParser
    {
        public static TextWriter Log;
        private readonly Document document;
        private readonly BlockParserState blockParserState;
        private readonly StringBuilderCache stringBuilderCache;
        private readonly InlineParserState inlineState;

        public MarkdownParser(TextReader reader)
        {
            document = new Document();
            Reader = reader;
            stringBuilderCache = new StringBuilderCache();

            // TODO: Make this configurable outside here
            var blockParsers = new ParserList<BlockParser>()
            {
                new ThematicBreakParser(),
                new HeadingBlockParser(),
                new QuoteBlockParser(),
                new ListBlockParser(),

                new HtmlBlockParser(),
                new FencedCodeBlockParser(),
                new IndentedCodeBlockParser(),
                new ParagraphBlockParser(),
            };
            blockParsers.Initialize();

            var inlineParsers = new ParserList<InlineParser>()
            {
                HtmlEntityParser.Default,
                LinkInlineParser.Default,
                EmphasisInlineParser.Default,
                EscapeInlineParser.Default,
                CodeInlineParser.Default,
                AutolineInlineParser.Default,
                HardlineBreakInlineParser.Default,
            };
            inlineParsers.Initialize();


            blockParserState = new BlockParserState(stringBuilderCache, document, blockParsers);
            inlineState = new InlineParserState(stringBuilderCache, document, inlineParsers) {Log = Log};
        }

        public TextReader Reader { get; }

        public Document Parse()
        {
            ParseLines();
            ProcessInlines(document);
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

        //private void ProcessInlinesTasks(ContainerBlock container)
        //{
        //    var list = new Stack<ContainerBlock>();
        //    list.Push(container);
        //    var leafs = new List<Task>();

        //    while (list.Count > 0)
        //    {
        //        container = list.Pop();
        //        foreach (var block in container.Children)
        //        {
        //            var leafBlock = block as LeafBlock;
        //            if (leafBlock != null)
        //            {
        //                if (leafBlock.ProcessInlines)
        //                {
        //                    var task = new Task(() => ProcessInlineLeaf(leafBlock));
        //                    task.Start();
        //                    leafs.Add(task);
        //                    //ProcessInlineLeaf(leafBlock);
        //                }
        //            }
        //            else 
        //            {
        //                list.Push((ContainerBlock)block);
        //            }
        //        }
        //    }

        //    Task.WaitAll(leafs.ToArray());
        //}

        private void ProcessInlines(ContainerBlock container)
        {
            var list = new Stack<ContainerBlock>();
            list.Push(container);
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
                            inlineState.ProcessInlineLeaf(leafBlock);
                        }
                    }
                    else
                    {
                        list.Push((ContainerBlock)block);
                    }
                }
            }
        }
    }
}