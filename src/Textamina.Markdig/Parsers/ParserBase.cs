// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Base class for a <see cref="BlockParser"/> or <see cref="InlineParser"/>.
    /// </summary>
    /// <typeparam name="TParserState">Type of the parser state</typeparam>
    /// <seealso cref="Textamina.Markdig.Parsers.IMarkdownParser{TParserState}" />
    public class ParserBase<TParserState> : IMarkdownParser<TParserState>
    {
        /// <summary>
        /// Gets the opening characters this parser will be triggered if the character is found.
        /// </summary>
        public char[] OpeningCharacters { get; set; }

        /// <summary>
        /// Initializes this parser with the specified parser state.
        /// </summary>
        /// <param name="state">The parser state.</param>
        public virtual void Initialize(TParserState state)
        {
        }

        /// <summary>
        /// Gets the index of this parser in <see cref="T:Textamina.Markdig.Parsers.BlockParserList" /> or <see cref="T:Textamina.Markdig.Parsers.InlineParserList" />.
        /// </summary>
        public int Index { get; internal set; }
    }
}