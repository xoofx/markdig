// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Base interface for a block or inline parser.
    /// </summary>
    /// <typeparam name="TParserState">The type of the parser state.</typeparam>
    public interface IMarkdownParser<in TParserState>
    {
        /// <summary>
        /// Gets the opening characters this parser will be triggered if the character is found.
        /// </summary>
        char[] OpeningCharacters { get; }

        /// <summary>
        /// Initializes this parser with the specified parser state.
        /// </summary>
        /// <param name="state">The parser state.</param>
        void Initialize(TParserState state);

        /// <summary>
        /// Gets the index of this parser in <see cref="BlockParserList"/> or <see cref="InlineParserList"/>.
        /// </summary>
        int Index { get; }
    }
}