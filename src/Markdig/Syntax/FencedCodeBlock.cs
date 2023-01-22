// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax;

/// <summary>
/// Represents a fenced code block.
/// </summary>
/// <remarks>
/// Related to CommonMark spec: 4.5 Fenced code blocks
/// </remarks>
public class FencedCodeBlock : CodeBlock, IFencedBlock
{
    private TriviaProperties? _trivia => TryGetDerivedTrivia<TriviaProperties>();
    private TriviaProperties Trivia => GetOrSetDerivedTrivia<TriviaProperties>();

    /// <summary>
    /// Initializes a new instance of the <see cref="FencedCodeBlock"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    public FencedCodeBlock(BlockParser parser) : base(parser)
    {
        // Fenced code blocks are not breakable, unless
        // we reach:
        // - a fenced line terminator
        // - the closing of the container that is holding this fenced block
        IsBreakable = false;
    }

    /// <summary>
    /// Gets or sets the indent count when the fenced code block was indented
    /// and we need to remove up to indent count chars spaces from the beginning of a line.
    /// </summary>
    public int IndentCount { get; set; }

    /// <inheritdoc />
    public char FencedChar { get; set; }

    /// <inheritdoc />
    public int OpeningFencedCharCount { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaAfterFencedChar { get => _trivia?.TriviaAfterFencedChar ?? StringSlice.Empty; set => Trivia.TriviaAfterFencedChar = value; }

    /// <inheritdoc />
    public string? Info { get; set; }

    /// <inheritdoc />
    public StringSlice UnescapedInfo { get => _trivia?.UnescapedInfo ?? StringSlice.Empty; set => Trivia.UnescapedInfo = value; }

    /// <inheritdoc />
    public StringSlice TriviaAfterInfo { get => _trivia?.TriviaAfterInfo ?? StringSlice.Empty; set => Trivia.TriviaAfterInfo = value; }

    /// <inheritdoc />
    public string? Arguments { get; set; }

    /// <inheritdoc />
    public StringSlice UnescapedArguments { get => _trivia?.UnescapedArguments ?? StringSlice.Empty; set => Trivia.UnescapedArguments = value; }

    /// <inheritdoc />
    public StringSlice TriviaAfterArguments { get => _trivia?.TriviaAfterArguments ?? StringSlice.Empty; set => Trivia.TriviaAfterArguments = value; }

    /// <inheritdoc />
    public NewLine InfoNewLine { get => _trivia?.InfoNewLine ?? NewLine.None; set => Trivia.InfoNewLine = value; }

    /// <inheritdoc />
    public StringSlice TriviaBeforeClosingFence { get => _trivia?.TriviaBeforeClosingFence ?? StringSlice.Empty; set => Trivia.TriviaBeforeClosingFence = value; }

    /// <inheritdoc />
    public int ClosingFencedCharCount { get; set; }

    private sealed class TriviaProperties
    {
        public StringSlice TriviaAfterFencedChar;
        public StringSlice UnescapedInfo;
        public StringSlice TriviaAfterInfo;
        public StringSlice UnescapedArguments;
        public StringSlice TriviaAfterArguments;
        public NewLine InfoNewLine;
        public StringSlice TriviaBeforeClosingFence;
    }
}