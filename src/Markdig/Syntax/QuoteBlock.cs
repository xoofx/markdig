// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax;

/// <summary>
/// A block quote (Section 5.1 CommonMark specs)
/// </summary>
/// <seealso cref="ContainerBlock" />
public class QuoteBlock : ContainerBlock
{
    private List<QuoteBlockLine> Trivia => GetOrSetDerivedTrivia<List<QuoteBlockLine>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteBlock"/> class.
    /// </summary>
    /// <param name="parser">The parser used to create this block.</param>
    public QuoteBlock(BlockParser? parser) : base(parser)
    {
    }

    /// <summary>
    /// Gets or sets the trivia per line of this QuoteBlock.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled.
    /// </summary>
    public List<QuoteBlockLine> QuoteLines => Trivia;

    /// <summary>
    /// Gets or sets the quote character (usually `&gt;`)
    /// </summary>
    public char QuoteChar { get; set; }
}

/// <summary>
/// Represents trivia per line part of a QuoteBlock.
/// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled.
/// </summary>
public class QuoteBlockLine
{
    /// <summary>
    /// Gets or sets trivia occurring before the first quote character.
    /// </summary>
    public StringSlice TriviaBefore { get; set; }

    /// <summary>
    /// True when this QuoteBlock line has a quote character. False when
    /// this line is a "lazy line".
    /// </summary>
    public bool QuoteChar { get; set; }

    /// <summary>
    /// True if a space is parsed right after the quote character.
    /// </summary>
    public bool HasSpaceAfterQuoteChar { get; set; }

    /// <summary>
    /// Gets or sets the trivia after the the space after the quote character.
    /// The first space is assigned to <see cref="HasSpaceAfterQuoteChar"/>, subsequent
    /// trivia is assigned to this property.
    /// </summary>
    public StringSlice TriviaAfter { get; set; }

    /// <summary>
    /// Gets or sets the newline of this QuoeBlockLine.
    /// </summary>
    public NewLine NewLine { get; set; }
}