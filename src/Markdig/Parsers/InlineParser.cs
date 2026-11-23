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
    /// Gets a value indicating whether <see cref="PrecheckMatch"/> can rule out a match of this parser.
    /// </summary>
    internal virtual bool HasMatchPrecheck => false;

    /// <summary>
    /// Returns <c>false</c> if <see cref="Match"/> cannot succeed at the beginning of the slice. Must not have side effects.
    /// </summary>
    internal virtual bool PrecheckMatch(in StringSlice slice) => true;
}