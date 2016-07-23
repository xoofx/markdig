// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Parsers
{
    /// <summary>
    /// Base class for a <see cref="BlockParser"/> or <see cref="InlineParser"/>.
    /// </summary>
    /// <typeparam name="TProcessor">Type of the parser processor</typeparam>
    /// <seealso cref="Markdig.Parsers.IMarkdownParser{TParserState}" />
    public abstract class ParserBase<TProcessor> : IMarkdownParser<TProcessor>
    {
        /// <summary>
        /// Gets the opening characters this parser will be triggered if the character is found.
        /// </summary>
        public char[] OpeningCharacters { get; set; }

        /// <summary>
        /// Initializes this parser with the specified parser processor.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Gets the index of this parser in <see cref="T:Markdig.Parsers.BlockParserList" /> or <see cref="T:Markdig.Parsers.InlineParserList" />.
        /// </summary>
        public int Index { get; internal set; }
    }
}