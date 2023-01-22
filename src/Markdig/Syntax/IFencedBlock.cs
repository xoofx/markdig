// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Syntax;

/// <summary>
/// A common interface for fenced block (e.g: <see cref="FencedCodeBlock"/> or <see cref="Extensions.CustomContainers.CustomContainer"/>)
/// </summary>
public interface IFencedBlock : IBlock
{
    /// <summary>
    /// Gets or sets the fenced character used to open and close this fenced code block.
    /// </summary>
    char FencedChar { get; set; }

    /// <summary>
    /// Gets or sets the fenced character count used to open this fenced code block.
    /// </summary>
    int OpeningFencedCharCount { get; set; }

    /// <summary>
    /// Gets or sets the trivia after the <see cref="FencedChar"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    StringSlice TriviaAfterFencedChar { get; set; }

    /// <summary>
    /// Gets or sets the language parsed after the first line of 
    /// the fenced code block. May be null.
    /// </summary>
    string? Info { get; set; }

    /// <summary>
    /// Non-escaped <see cref="Info"/> exactly as in source markdown.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    StringSlice UnescapedInfo { get; set; }

    /// <summary>
    /// Gets or sets the trivia after the <see cref="Info"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    StringSlice TriviaAfterInfo { get; set; }

    /// <summary>
    /// Gets or sets the arguments after the <see cref="Info"/>.
    /// May be null.
    /// </summary>
    string? Arguments { get; set; }

    /// <summary>
    /// Non-escaped <see cref="Arguments"/> exactly as in source markdown.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    StringSlice UnescapedArguments { get; set; }

    /// <summary>
    /// Gets or sets the trivia after the <see cref="Arguments"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    StringSlice TriviaAfterArguments { get; set; }

    /// <summary>
    /// Newline of the line with the opening fenced chars.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="NewLine.None"/>.
    /// </summary>
    NewLine InfoNewLine { get; set; }

    /// <summary>
    /// Trivia before the closing fenced chars
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    StringSlice TriviaBeforeClosingFence { get; set; }

    /// <summary>
    /// Gets or sets the fenced character count used to close this fenced code block.
    /// </summary>
    int ClosingFencedCharCount { get; set; }

    /// <summary>
    /// Newline after the last line, which is always the line containing the closing fence chars.
    /// "Inherited" from <see cref="Block.NewLine"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="NewLine.None"/>.
    /// </summary>
    NewLine NewLine { get; set; }
}