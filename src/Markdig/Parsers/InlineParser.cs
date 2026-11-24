// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Parsers;

/// <summary>
/// Base class for parsing an <see cref="Syntax.Inlines.Inline"/>.
/// </summary>
/// <seealso cref="InlineProcessor" />
public abstract class InlineParser : ParserBase<InlineProcessor>, IInlineParser<InlineProcessor>
{
    /// <summary>
    /// Tries to match the specified slice.
    /// </summary>
    /// <param name="processor">The parser processor.</param>
    /// <param name="slice">The text slice.</param>
    /// <returns><c>true</c> if this parser found a match; <c>false</c> otherwise</returns>
    public abstract bool Match(InlineProcessor processor, ref StringSlice slice);

    /// <summary>
    /// Gets a value indicating whether <see cref="IndexOfPossibleMatch"/> finds every position at which this parser
    /// can match.
    /// </summary>
    internal virtual bool CanSearchPossibleMatch => false;

    /// <summary>
    /// Returns the first position from <paramref name="start"/> to <paramref name="end"/> at which <see cref="Match"/>
    /// may succeed, or -1 if there is none. The result must depend on the text alone.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    internal virtual int IndexOfPossibleMatch(string text, int start, int end) => start;
}