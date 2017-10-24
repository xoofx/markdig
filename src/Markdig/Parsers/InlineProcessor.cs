// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using Markdig.Helpers;
using Markdig.Parsers.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers
{
    /// <summary>
    /// A delegate called at inline processing stage.
    /// </summary>
    /// <param name="processor">The processor.</param>
    /// <param name="inline">The inline being processed.</param>
    public delegate void ProcessInlineDelegate(InlineProcessor processor, Inline inline);

    /// <summary>
    /// The inline parser state used by all <see cref="InlineParser"/>.
    /// </summary>
    public class InlineProcessor
    {
        private readonly List<StringLineGroup.LineOffset> lineOffsets;
        private int previousSliceOffset;
        private int previousLineIndexForSliceOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineProcessor" /> class.
        /// </summary>
        /// <param name="stringBuilders">The string builders.</param>
        /// <param name="document">The document.</param>
        /// <param name="parsers">The parsers.</param>
        /// <param name="inlineCreated">The inline created event.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public InlineProcessor(StringBuilderCache stringBuilders, MarkdownDocument document, InlineParserList parsers, bool preciseSourcelocation)
        {
            if (stringBuilders == null) throw new ArgumentNullException(nameof(stringBuilders));
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (parsers == null) throw new ArgumentNullException(nameof(parsers));
            StringBuilders = stringBuilders;
            Document = document;
            Parsers = parsers;
            PreciseSourceLocation = preciseSourcelocation;
            lineOffsets = new List<StringLineGroup.LineOffset>();
            ParserStates = new object[Parsers.Count];
            LiteralInlineParser = new LiteralInlineParser();
        }

        /// <summary>
        /// Gets the current block being proessed.
        /// </summary>
        public LeafBlock Block { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to provide precise source location.
        /// </summary>
        public bool PreciseSourceLocation { get; }

        /// <summary>
        /// Gets or sets the new block to replace the block being processed.
        /// </summary>
        public Block BlockNew { get; set; }

        /// <summary>
        /// Gets or sets the current inline. Used by <see cref="InlineParser"/> to return a new inline if match was successfull
        /// </summary>
        public Inline Inline { get; set; }

        /// <summary>
        /// Gets the root container of the current <see cref="Block"/>.
        /// </summary>
        public ContainerInline Root { get; internal set; }

        /// <summary>
        /// Gets the list of inline parsers.
        /// </summary>
        public InlineParserList Parsers { get; }

        /// <summary>
        /// Gets the root document.
        /// </summary>
        public MarkdownDocument Document { get; }

        /// <summary>
        /// Gets the cache string builders.
        /// </summary>
        public StringBuilderCache StringBuilders { get;  }

        /// <summary>
        /// Gets or sets the index of the line from the begining of the document being processed.
        /// </summary>
        public int LineIndex { get; private set; }

        /// <summary>
        /// Gets the parser states that can be used by <see cref="InlineParser"/> using their <see cref="InlineParser.Index"/> property.
        /// </summary>
        public object[] ParserStates { get; }

        /// <summary>
        /// Gets or sets the debug log writer. No log if null.
        /// </summary>
        public TextWriter DebugLog { get; set; }

        /// <summary>
        /// Gets the literal inline parser.
        /// </summary>
        public LiteralInlineParser LiteralInlineParser { get; }


        public int GetSourcePosition(int sliceOffset)
        {
            int column;
            int lineIndex;
            return GetSourcePosition(sliceOffset, out lineIndex, out column);
        }

        public SourceSpan GetSourcePositionFromLocalSpan(SourceSpan span)
        {
            if (span.IsEmpty)
            {
                return SourceSpan.Empty;
            }

            int column;
            int lineIndex;
            return new SourceSpan(GetSourcePosition(span.Start, out lineIndex, out column), GetSourcePosition(span.End, out lineIndex, out column));
        }

        /// <summary>
        /// Gets the source position for the specified offset within the current slice.
        /// </summary>
        /// <param name="sliceOffset">The slice offset.</param>
        /// <returns>The source position</returns>
        public int GetSourcePosition(int sliceOffset, out int lineIndex, out int column)
        {
            column = 0;            
            lineIndex = sliceOffset >= previousSliceOffset ? previousLineIndexForSliceOffset : 0;
            int position = 0;
            if (PreciseSourceLocation)
            {
                for (; lineIndex < lineOffsets.Count; lineIndex++)
                {
                    var lineOffset = lineOffsets[lineIndex];
                    if (sliceOffset <= lineOffset.End)
                    {
                        // Use the beginning of the line as a previous slice offset 
                        // (since it is on the same line)
                        previousSliceOffset = lineOffsets[lineIndex].Start;
                        var delta = sliceOffset - previousSliceOffset;
                        column = lineOffsets[lineIndex].Column + delta;
                        position = lineOffset.LinePosition + delta + lineOffsets[lineIndex].Offset;
                        previousLineIndexForSliceOffset = lineIndex;

                        // Return an absolute line index
                        lineIndex = lineIndex + LineIndex;
                        break;
                    }
                }
            }
            return position;
        }

        /// <summary>
        /// Processes the inline of the specified <see cref="LeafBlock"/>.
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        public void ProcessInlineLeaf(LeafBlock leafBlock)
        {
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));

            // clear parser states
            Array.Clear(ParserStates, 0, ParserStates.Length);

            Root = new ContainerInline() { IsClosed = false };
            leafBlock.Inline = Root;
            Inline = null;
            Block = leafBlock;
            BlockNew = null;
            LineIndex = leafBlock.Line;

            previousSliceOffset = 0;
            previousLineIndexForSliceOffset = 0;
            lineOffsets.Clear();
            var text = leafBlock.Lines.ToSlice(lineOffsets);
            leafBlock.Lines = new StringLineGroup();

            int previousStart = -1;

            while (!text.IsEmpty)
            {
                // Security check so that the parser can't go into a crazy infinite loop if one extension is messing
                if (previousStart == text.Start)
                {
                    throw new InvalidOperationException($"The parser is in an invalid infinite loop while trying to parse inlines for block [{leafBlock.GetType().Name}] at position ({leafBlock.ToPositionText()}");
                }
                previousStart = text.Start;

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
                LiteralInlineParser.Match(this, ref text);

                done:
                var nextInline = Inline;
                if (nextInline != null)
                {
                    if (nextInline.Parent == null)
                    {
                        // Get deepest container
                        var container = FindLastContainer();
                        if (!ReferenceEquals(container, nextInline))
                        {
                            container.AppendChild(nextInline);
                        }

                        if (container == Root)
                        {
                            if (container.Span.IsEmpty)
                            {
                                container.Span = nextInline.Span;
                            }
                            container.Span.End = nextInline.Span.End;
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

                //if (DebugLog != null)
                //{
                //    DebugLog.WriteLine($"** Dump: char '{c}");
                //    leafBlock.Inline.DumpTo(DebugLog);
                //}
            }

            Inline = null;
            //if (DebugLog != null)
            //{
            //    DebugLog.WriteLine("** Dump before Emphasis:");
            //    leafBlock.Inline.DumpTo(DebugLog);
            //}

            // PostProcess all inlines
            PostProcessInlines(0, Root, null, true);

            //TransformDelimitersToLiterals();

            //if (DebugLog != null)
            //{
            //    DebugLog.WriteLine();
            //    DebugLog.WriteLine("** Dump after Emphasis:");
            //    leafBlock.Inline.DumpTo(DebugLog);
            //}
        }

        public void PostProcessInlines(int startingIndex, Inline root, Inline lastChild, bool isFinalProcessing)
        {
            for (int i = startingIndex; i < Parsers.PostInlineProcessors.Length; i++)
            {
                var postInlineProcessor = Parsers.PostInlineProcessors[i];
                if (!postInlineProcessor.PostProcess(this, root, lastChild, i, isFinalProcessing))
                {
                    break;
                }
            }
        }

        private ContainerInline FindLastContainer()
        {
            var container = Block.Inline;
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