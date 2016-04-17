// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;

namespace Markdig.Parsers
{
    /// <summary>
    /// Base interface for parsing an <see cref="Syntax.Inlines.Inline"/>.
    /// </summary>
    /// <seealso cref="InlineParser" />
    /// <seealso cref="InlineProcessor" />
    public interface IInlineParser<in TProcessor> : IMarkdownParser<TProcessor>
    {
        /// <summary>
        /// Tries to match the specified slice.
        /// </summary>
        /// <param name="processor">The parser processor.</param>
        /// <param name="slice">The text slice.</param>
        /// <returns><c>true</c> if this parser found a match; <c>false</c> otherwise</returns>
        bool Match(InlineProcessor processor, ref StringSlice slice);
    }
}