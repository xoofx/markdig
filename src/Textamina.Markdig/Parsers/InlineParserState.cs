// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// The inline parser state used by all <see cref="InlineParser"/>.
    /// </summary>
    public class InlineParserState
    {
        internal static TextWriter Log;

        private readonly List<int> lineOffsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineParserState"/> class.
        /// </summary>
        /// <param name="stringBuilders">The string builders.</param>
        /// <param name="document">The document.</param>
        /// <param name="parsers">The parsers.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
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
            lineOffsets = new List<int>();
            Parsers.Initialize(this);
            ParserStates = new object[Parsers.Count];
        }

        public LeafBlock Block { get; private set; }

        public Block BlockNew { get; set; }

        public Inline Inline { get; set; }

        public ContainerInline Root { get; internal set; }

        public List<Inline> InlinesToClose { get; }

        public InlineParserList Parsers { get; }

        public Document Document { get; }

        public StringBuilderCache StringBuilders { get;  }

        public int LineIndex { get; set; }

        public int LocalLineIndex { get; set; }

        public char[] SpecialCharacters { get; set; }

        public object[] ParserStates { get; }

        /// <summary>
        /// Processes the inline of the specified <see cref="LeafBlock"/>.
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        public void ProcessInlineLeaf(LeafBlock leafBlock)
        {
            // clear parser states
            Array.Clear(ParserStates, 0, ParserStates.Length);

            Root = new ContainerInline() { IsClosed = false };
            leafBlock.Inline = Root;
            Inline = null;
            Block = leafBlock;
            BlockNew = null;
            LineIndex = leafBlock.Line;

            lineOffsets.Clear();
            LocalLineIndex = 0;
            var text = leafBlock.Lines.ToSlice(lineOffsets);
            leafBlock.Lines = null;

            while (!text.IsEmpty)
            {
                var c = text.CurrentChar;

                // Update line index
                if (text.Start >= lineOffsets[LocalLineIndex])
                {
                    LineIndex++;
                    LocalLineIndex++;
                }

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
                    if (Inline == Root)
                    {
                        Inline = null;
                    }
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

            // Process all delimiters
            ProcessDelimiters(0, Root);

            //TransformDelimitersToLiterals();

            if (Log != null)
            {
                Log.WriteLine();
                Log.WriteLine("** Dump after Emphasis:");
                leafBlock.Inline.DumpTo(Log);
            }
        }

        public void ProcessDelimiters(int startingIndex, Inline root, Inline lastChild = null)
        {
            for (int i = startingIndex; i < Parsers.DelimiterProcessors.Length; i++)
            {
                var delimiterProcessor = Parsers.DelimiterProcessors[i];
                if (!delimiterProcessor.ProcessDelimiters(this, root, lastChild, i))
                {
                    break;
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