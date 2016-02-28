using System;
using System.Collections.Generic;
using System.IO;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers
{
    public class InlineParserState
    {
        public InlineParserState(StringBuilderCache stringBuilders, Document document, InlineParserList parsers)
        {
            if (stringBuilders == null) throw new ArgumentNullException(nameof(stringBuilders));
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (parsers == null) throw new ArgumentNullException(nameof(parsers));
            StringBuilders = stringBuilders;
            Document = document;
            InlinesToClose = new List<Inline>();
            Parsers = parsers;
            SpecialCharacters = Parsers.OpeningCharacters;
            Parsers.Initialize(this);
        }

        public LeafBlock Block { get; internal set; }

        public Inline Inline { get; set; }

        public ContainerInline Root { get; internal set; }

        public readonly List<Inline> InlinesToClose;

        public readonly InlineParserList Parsers;

        public readonly Document Document;

        public StringBuilderCache StringBuilders { get;  }

        public TextWriter Log;

        public int LineIndex { get; private set; }

        public char[] SpecialCharacters { get; set; }

        public void ProcessInlineLeaf(LeafBlock leafBlock)
        {
            this.Root = new ContainerInline() { IsClosed = false };
            leafBlock.Inline = Root;
            this.Inline = leafBlock.Inline;
            this.Block = leafBlock;

            var text = leafBlock.ToInlineSlice();
            leafBlock.Lines = null;

            while (!text.IsEmpty)
            {
                var c = text.CurrentChar;
                var textSaved = text;
                var parsers = Parsers.GetParsersForOpeningCharacter(c);
                if (parsers != null)
                {
                    for (int i = 0; i < parsers.Length; i++)
                    {
                        text = textSaved;
                        if (parsers[i].Match(this, ref text))
                        {
                            goto done;
                        }
                    }
                }
                parsers = Parsers.GlobalParsers;
                if (parsers != null)
                {
                    for (int i = 0; i < parsers.Length; i++)
                    {
                        text = textSaved;
                        if (parsers[i].Match(this, ref text))
                        {
                            goto done;
                        }
                    }
                }

                text = textSaved;
                // Else match using the default literal inline parser
                LiteralInlineParser.Default.Match(this, ref text);

                done:
                var nextInline = Inline;
                if (nextInline != null)
                {
                    if (nextInline.Parent == null)
                    {
                        // Get deepest container
                        FindLastContainer().AppendChild(nextInline);
                    }

                    if (nextInline.IsClosable && !nextInline.IsClosed)
                    {
                        var inlinesToClose = InlinesToClose;
                        var last = inlinesToClose.Count > 0
                            ? InlinesToClose[inlinesToClose.Count - 1]
                            : null;
                        if (last != nextInline)
                        {
                            InlinesToClose.Add(nextInline);
                        }
                    }
                }
                else
                {
                    // Get deepest container
                    var container = FindLastContainer();

                    Inline = container.LastChild is LeafInline ? container.LastChild : container;
                }

                if (Log != null)
                {
                    Log.WriteLine($"** Dump: char '{c}");
                    leafBlock.Inline.DumpTo(Log);
                }
            }

            // Close all inlines not closed
            Inline = null;
            foreach (var inline in InlinesToClose)
            {
                inline.CloseInternal(this);
            }
            InlinesToClose.Clear();

            if (Log != null)
            {
                Log.WriteLine("** Dump before Emphasis:");
                leafBlock.Inline.DumpTo(Log);
            }

            // Process delimiters
            foreach (var delimiterProcessor in Parsers.DelimiterProcessors)
            {
                delimiterProcessor.ProcessDelimiters(Root, null);
            }
            TransformDelimitersToLiterals();

            if (Log != null)
            {
                Log.WriteLine();
                Log.WriteLine("** Dump after Emphasis:");
                leafBlock.Inline.DumpTo(Log);
            }
        }

        private void TransformDelimitersToLiterals()
        {
            var child = Root.LastChild;
            while (child != null)
            {
                var subContainer = child as ContainerInline;
                child = subContainer?.LastChild;
                var delimiterInline = subContainer as DelimiterInline;
                if (delimiterInline != null)
                {
                    delimiterInline.ReplaceBy(new LiteralInline() { Content = new StringSlice(delimiterInline.ToLiteral()), IsClosed = true });
                }
            }
        }

        private ContainerInline FindLastContainer()
        {
            var container = (ContainerInline)Block.Inline;
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
            return container;
        }
    }
}