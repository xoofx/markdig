// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax;

/// <summary>
/// Base class for a block structure. Either a <see cref="LeafBlock"/> or a <see cref="ContainerBlock"/>.
/// </summary>
/// <seealso cref="MarkdownObject" />
public abstract class Block : MarkdownObject, IBlock
{
    private BlockTriviaProperties? _trivia => GetTrivia<BlockTriviaProperties>();
    private BlockTriviaProperties Trivia => GetOrSetTrivia<BlockTriviaProperties>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Block"/> class.
    /// </summary>
    /// <param name="parser">The parser used to create this block.</param>
    protected Block(BlockParser? parser)
    {
        Parser = parser;
        IsOpen = true;
        IsBreakable = true;
        SetTypeKind(isInline: false, isContainer: false);
    }

    /// <summary>
    /// Gets the parent of this container. May be null.
    /// </summary>
    public ContainerBlock? Parent { get; internal set; }

    /// <summary>
    /// Gets the parser associated to this instance.
    /// </summary>
    public BlockParser? Parser { get; }

    internal bool IsLeafBlock { get; private protected set; }

    internal bool IsParagraphBlock { get; private protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is still open.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this block is breakable. Default is true.
    /// </summary>
    public bool IsBreakable { get; set; }

    /// <summary>
    /// The last newline of this block.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled
    /// </summary>
    public NewLine NewLine { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this block must be removed from its container after inlines have been processed.
    /// </summary>
    public bool RemoveAfterProcessInlines { get; set; }

    /// <summary>
    /// Gets or sets the trivia right before this block.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice TriviaBefore { get => _trivia?.TriviaBefore ?? StringSlice.Empty; set => Trivia.TriviaBefore = value; }

    /// <summary>
    /// Gets or sets trivia occurring after this block.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice TriviaAfter { get => _trivia?.TriviaAfter ?? StringSlice.Empty; set => Trivia.TriviaAfter = value; }

    /// <summary>
    /// Gets or sets the empty lines occurring before this block.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise null.
    /// </summary>
    public List<StringSlice>? LinesBefore { get => _trivia?.LinesBefore; set => Trivia.LinesBefore = value; }

    /// <summary>
    /// Gets or sets the empty lines occurring after this block.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise null.
    /// </summary>
    public List<StringSlice>? LinesAfter { get => _trivia?.LinesAfter; set => Trivia.LinesAfter = value; }

    /// <summary>
    /// Occurs when the process of inlines begin.
    /// </summary>
    public event ProcessInlineDelegate? ProcessInlinesBegin;

    /// <summary>
    /// Occurs when the process of inlines ends for this instance.
    /// </summary>
    public event ProcessInlineDelegate? ProcessInlinesEnd;

    /// <summary>
    /// Called when the process of inlines begin.
    /// </summary>
    /// <param name="state">The inline parser state.</param>
    internal void OnProcessInlinesBegin(InlineProcessor state)
    {
        ProcessInlinesBegin?.Invoke(state, null);
    }

    /// <summary>
    /// Called when the process of inlines ends.
    /// </summary>
    /// <param name="state">The inline parser state.</param>
    internal void OnProcessInlinesEnd(InlineProcessor state)
    {
        ProcessInlinesEnd?.Invoke(state, null);
    }

    public void UpdateSpanEnd(int spanEnd)
    {
        // Update parent spans
        int depth = 0;
        var parent = this;
        while (parent != null)
        {
            if (spanEnd > parent.Span.End)
            {
                parent.Span.End = spanEnd;
            }
            parent = parent.Parent;
            depth++;
        }
        ThrowHelper.CheckDepthLimit(depth, useLargeLimit: true);
    }

    internal static Block FindRootMostContainerParent(Block block)
    {
        while (true)
        {
            Block? parent = block.Parent;
            if (parent is null || !parent.IsContainerBlock || parent is MarkdownDocument)
            {
                break;
            }
            block = parent;
        }
        return block;
    }

    private protected T? TryGetDerivedTrivia<T>() where T : class => _trivia?.DerivedTriviaSlot as T;
    private protected T GetOrSetDerivedTrivia<T>() where T : new() => (T)(Trivia.DerivedTriviaSlot ??= new T());

    private sealed class BlockTriviaProperties
    {
        // Used by derived types to store their own TriviaProperties
        public object? DerivedTriviaSlot;

        public StringSlice TriviaBefore;
        public StringSlice TriviaAfter;
        public List<StringSlice>? LinesBefore;
        public List<StringSlice>? LinesAfter;
    }
}