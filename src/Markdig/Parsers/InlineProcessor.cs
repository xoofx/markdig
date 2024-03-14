// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Markdig.Helpers;
using Markdig.Parsers.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers;

/// <summary>
/// A delegate called at inline processing stage.
/// </summary>
/// <param name="processor">The processor.</param>
/// <param name="inline">The inline being processed.</param>
public delegate void ProcessInlineDelegate(InlineProcessor processor, Inline? inline);

/// <summary>
/// The inline parser state used by all <see cref="InlineParser"/>.
/// </summary>
public class InlineProcessor
{
    private readonly List<StringLineGroup.LineOffset> lineOffsets = [];
    private int previousSliceOffset;
    private int previousLineIndexForSliceOffset;
    internal ContainerBlock? PreviousContainerToReplace;
    internal ContainerBlock? NewContainerToReplace;

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineProcessor" /> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="parsers">The parsers.</param>
    /// <param name="preciseSourcelocation">A value indicating whether to provide precise source location.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <param name="trackTrivia">Whether to parse trivia such as whitespace, extra heading characters and unescaped string values.</param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    public InlineProcessor(MarkdownDocument document, InlineParserList parsers, bool preciseSourcelocation, MarkdownParserContext? context, bool trackTrivia = false)
    {
        Setup(document, parsers, preciseSourcelocation, context, trackTrivia);
    }

    private InlineProcessor() { }

    /// <summary>
    /// Gets the current block being processed.
    /// </summary>
    public LeafBlock? Block { get; private set; }

    /// <summary>
    /// Gets a value indicating whether to provide precise source location.
    /// </summary>
    public bool PreciseSourceLocation { get; private set; }

    /// <summary>
    /// Gets or sets the new block to replace the block being processed.
    /// </summary>
    public Block? BlockNew { get; set; }

    /// <summary>
    /// Gets or sets the current inline. Used by <see cref="InlineParser"/> to return a new inline if match was successfull
    /// </summary>
    public Inline? Inline { get; set; }

    /// <summary>
    /// Gets the root container of the current <see cref="Block"/>.
    /// </summary>
    public ContainerInline? Root { get; internal set; }

    /// <summary>
    /// Gets the list of inline parsers.
    /// </summary>
    public InlineParserList Parsers { get; private set; } = null!; // Set in Setup

    /// <summary>
    /// Gets the parser context or <c>null</c> if none is available.
    /// </summary>
    public MarkdownParserContext? Context { get; private set; }

    /// <summary>
    /// Gets the root document.
    /// </summary>
    public MarkdownDocument Document { get; private set; } = null!; // Set in Setup

    /// <summary>
    /// Gets or sets the index of the line from the begining of the document being processed.
    /// </summary>
    public int LineIndex { get; private set; }

    /// <summary>
    /// Gets the parser states that can be used by <see cref="InlineParser"/> using their <see cref="ParserBase{Inline}.Index"/> property.
    /// </summary>
    public object[] ParserStates { get; private set; } = null!; // Set in Setup

    /// <summary>
    /// Gets or sets the debug log writer. No log if null.
    /// </summary>
    public TextWriter? DebugLog { get; set; }

    /// <summary>
    /// True to parse trivia such as whitespace, extra heading characters and unescaped
    /// string values.
    /// </summary>
    public bool TrackTrivia { get; private set; }

    /// <summary>
    /// Gets the literal inline parser.
    /// </summary>
    public LiteralInlineParser LiteralInlineParser { get; } = new();

    public SourceSpan GetSourcePositionFromLocalSpan(SourceSpan span)
    {
        if (span.IsEmpty)
        {
            return SourceSpan.Empty;
        }

        return new SourceSpan(GetSourcePosition(span.Start), GetSourcePosition(span.End));
    }

    /// <summary>
    /// Gets the source position for the specified offset within the current slice.
    /// </summary>
    /// <param name="sliceOffset">The slice offset.</param>
    /// <param name="lineIndex">The line index.</param>
    /// <param name="column">The column.</param>
    /// <returns>The source position</returns>
    public int GetSourcePosition(int sliceOffset, out int lineIndex, out int column)
    {
        column = 0;
        lineIndex = sliceOffset >= previousSliceOffset ? previousLineIndexForSliceOffset : 0;
        int position = 0;
        if (PreciseSourceLocation)
        {
#if NET
            var offsets = CollectionsMarshal.AsSpan(lineOffsets);

            for (; (uint)lineIndex < (uint)offsets.Length; lineIndex++)
            {
                ref var lineOffset = ref offsets[lineIndex];
#else
            for (; lineIndex < lineOffsets.Count; lineIndex++)
            {
                var lineOffset = lineOffsets[lineIndex];
#endif

                if (sliceOffset <= lineOffset.End)
                {
                    // Use the beginning of the line as a previous slice offset
                    // (since it is on the same line)
                    previousSliceOffset = lineOffset.Start;
                    var delta = sliceOffset - previousSliceOffset;
                    column = lineOffset.Column + delta;
                    position = lineOffset.LinePosition + delta + lineOffset.Offset;
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
    /// Gets the source position for the specified offset within the current slice.
    /// </summary>
    /// <param name="sliceOffset">The slice offset.</param>
    /// <returns>The source position</returns>
    public int GetSourcePosition(int sliceOffset)
    {
        if (PreciseSourceLocation)
        {
            int lineIndex = sliceOffset >= previousSliceOffset ? previousLineIndexForSliceOffset : 0;

#if NET
            var offsets = CollectionsMarshal.AsSpan(lineOffsets);

            for (; (uint)lineIndex < (uint)offsets.Length; lineIndex++)
            {
                ref var lineOffset = ref offsets[lineIndex];
#else
            for (; lineIndex < lineOffsets.Count; lineIndex++)
            {
                var lineOffset = lineOffsets[lineIndex];
#endif

                if (sliceOffset <= lineOffset.End)
                {
                    previousLineIndexForSliceOffset = lineIndex;
                    previousSliceOffset = lineOffset.Start;

                    return sliceOffset - lineOffset.Start + lineOffset.LinePosition + lineOffset.Offset;
                }
            }
        }
        return 0;
    }

    /// <summary>
    /// Replace a parent container. This method is experimental and should be used with caution.
    /// </summary>
    /// <param name="previousParentContainer">The previous parent container to replace</param>
    /// <param name="newParentContainer">The new parent container</param>
    /// <exception cref="InvalidOperationException">If a new parent container has been already setup.</exception>
    internal void ReplaceParentContainer(ContainerBlock previousParentContainer, ContainerBlock newParentContainer)
    {
        // Limitation for now, only one parent container can be replaced.
        if (PreviousContainerToReplace != null)
        {
            throw new InvalidOperationException("A block is already being replaced");
        }

        PreviousContainerToReplace = previousParentContainer;
        NewContainerToReplace = newParentContainer;
    }

    /// <summary>
    /// Processes the inline of the specified <see cref="LeafBlock"/>.
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    public void ProcessInlineLeaf(LeafBlock leafBlock)
    {
        if (leafBlock is null) ThrowHelper.ArgumentNullException_leafBlock();

        PreviousContainerToReplace = null;
        NewContainerToReplace = null;

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
        var textEnd = text.End;
        leafBlock.Lines.Release();
        int previousStart = -1;

        while (!text.IsEmpty)
        {
            // Security check so that the parser can't go into a crazy infinite loop if one extension is messing
            if (previousStart == text.Start)
            {
                ThrowHelper.InvalidOperationException($"The parser is in an invalid infinite loop while trying to parse inlines for block [{leafBlock.GetType().Name}] at position ({leafBlock.ToPositionText()}");
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
                if (nextInline.Parent is null)
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

        if (TrackTrivia)
        {
            if (!(leafBlock is HeadingBlock))
            {
                var newLine = leafBlock.NewLine;
                if (newLine != NewLine.None)
                {
                    var position = GetSourcePosition(textEnd + 1, out int line, out int column);
                    leafBlock.Inline.AppendChild(new LineBreakInline { NewLine = newLine, Line = line, Column = column, Span = { Start = position, End = position + (newLine == NewLine.CarriageReturnLineFeed ? 1 : 0) } });
                }
            }
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

        if (leafBlock.Inline.LastChild is not null)
        {
            leafBlock.Inline.Span.End = leafBlock.Inline.LastChild.Span.End;
            leafBlock.UpdateSpanEnd(leafBlock.Inline.Span.End);
        }
    }

    public void PostProcessInlines(int startingIndex, Inline? root, Inline? lastChild, bool isFinalProcessing)
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
        var container = Block!.Inline!;
        for (int depth = 0; ; depth++)
        {
            Inline? lastChild = container.LastChild;
            if (lastChild is not null && lastChild.IsContainerInline && !lastChild.IsClosed)
            {
                container = Unsafe.As<ContainerInline>(lastChild);
            }
            else
            {
                ThrowHelper.CheckDepthLimit(depth, useLargeLimit: true);
                return container;
            }
        }
    }


    [MemberNotNull(nameof(Document), nameof(Parsers), nameof(ParserStates))]
    private void Setup(MarkdownDocument document, InlineParserList parsers, bool preciseSourcelocation, MarkdownParserContext? context, bool trackTrivia)
    {
        if (document is null) ThrowHelper.ArgumentNullException(nameof(document));
        if (parsers is null) ThrowHelper.ArgumentNullException(nameof(parsers));

        Document = document;
        Parsers = parsers;
        Context = context;
        PreciseSourceLocation = preciseSourcelocation;
        TrackTrivia = trackTrivia;

        if (ParserStates is null || ParserStates.Length < Parsers.Count)
        {
            ParserStates = new object[Parsers.Count];
        }
    }

    private void Reset()
    {
        Block = null;
        BlockNew = null;
        Inline = null;
        Root = null;
        Parsers = null!;
        Context = null;
        Document = null!;
        DebugLog = null;

        PreciseSourceLocation = false;
        TrackTrivia = false;

        LineIndex = 0;
        previousSliceOffset = 0;
        previousLineIndexForSliceOffset = 0;

        LiteralInlineParser.PostMatch = null;

        lineOffsets.Clear();
        Array.Clear(ParserStates, 0, ParserStates.Length);
    }

    private static readonly InlineProcessorCache _cache = new();

    internal static InlineProcessor Rent(MarkdownDocument document, InlineParserList parsers, bool preciseSourcelocation, MarkdownParserContext? context, bool trackTrivia)
    {
        var processor = _cache.Get();
        processor.Setup(document, parsers, preciseSourcelocation, context, trackTrivia);
        return processor;
    }

    internal static void Release(InlineProcessor processor)
    {
        _cache.Release(processor);
    }

    private sealed class InlineProcessorCache : ObjectCache<InlineProcessor>
    {
        protected override InlineProcessor NewInstance() => new InlineProcessor();

        protected override void Reset(InlineProcessor instance) => instance.Reset();
    }
}