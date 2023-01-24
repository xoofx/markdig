// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.CustomContainers;

/// <summary>
/// A block custom container.
/// </summary>
/// <seealso cref="ContainerBlock" />
/// <seealso cref="IFencedBlock" />
public class CustomContainer : ContainerBlock, IFencedBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomContainer"/> class.
    /// </summary>
    /// <param name="parser">The parser used to create this block.</param>
    public CustomContainer(BlockParser parser) : base(parser)
    {
    }

    /// <inheritdoc />
    public char FencedChar { get; set; }

    /// <inheritdoc />
    public int OpeningFencedCharCount { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaAfterFencedChar { get; set; }

    /// <inheritdoc />
    public string? Info { get; set; }

    /// <inheritdoc />
    public StringSlice UnescapedInfo { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaAfterInfo { get; set; }

    /// <inheritdoc />
    public string? Arguments { get; set; }

    /// <inheritdoc />
    public StringSlice UnescapedArguments { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaAfterArguments { get; set; }

    /// <inheritdoc />
    public NewLine InfoNewLine { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaBeforeClosingFence { get; set; }

    /// <inheritdoc />
    public int ClosingFencedCharCount { get; set; }
}