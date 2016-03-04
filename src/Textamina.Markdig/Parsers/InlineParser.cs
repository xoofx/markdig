// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Base class for parsing an <see cref="Syntax.Inlines.Inline"/>.
    /// </summary>
    /// <seealso cref="InlineParserState" />
    public abstract class InlineParser : ParserBase<InlineParserState>
    {
        /// <summary>
        /// Tries to match the specified slice.
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="slice">The text slice.</param>
        /// <returns><c>true</c> if this parser found a match; <c>false</c> otherwise</returns>
        public abstract bool Match(InlineParserState state, ref StringSlice slice);
    }
}