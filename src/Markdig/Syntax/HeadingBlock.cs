// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics;
using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax;

/// <summary>
/// Represents a heading.
/// </summary>
[DebuggerDisplay("{GetType().Name} Line: {Line}, {Lines} Level: {Level}")]
public class HeadingBlock : LeafBlock
{
    private TriviaProperties? _trivia => TryGetDerivedTrivia<TriviaProperties>();
    private TriviaProperties Trivia => GetOrSetDerivedTrivia<TriviaProperties>();

    /// <summary>
    /// Initializes a new instance of the <see cref="HeadingBlock"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    public HeadingBlock(BlockParser parser) : base(parser)
    {
        ProcessInlines = true;
    }

    /// <summary>
    /// Gets or sets the header character used to defines this heading (usually #)
    /// </summary>
    public char HeaderChar { get; set; }

    /// <summary>
    /// Gets or sets the level of heading (starting at 1 for the lowest level).
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// True if this heading is a Setext heading.
    /// </summary>
    public bool IsSetext { get; set; }

    /// <summary>
    /// Gets or sets the amount of - or = characters when <see cref="IsSetext"/> is true.
    /// </summary>
    public int HeaderCharCount { get; set; }

    /// <summary>
    /// Gets or sets the newline of the first line when <see cref="IsSetext"/> is true.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled.
    /// </summary>
    public NewLine SetextNewline { get => _trivia?.SetextNewline ?? NewLine.None; set => Trivia.SetextNewline = value; }

    /// <summary>
    /// Gets or sets the whitespace after the # character when <see cref="IsSetext"/> is false.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice TriviaAfterAtxHeaderChar { get => _trivia?.TriviaAfterAtxHeaderChar ?? StringSlice.Empty; set => Trivia.TriviaAfterAtxHeaderChar = value; }

    private sealed class TriviaProperties
    {
        public NewLine SetextNewline;
        public StringSlice TriviaAfterAtxHeaderChar;
    }
}