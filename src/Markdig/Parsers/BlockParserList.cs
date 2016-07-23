// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;

namespace Markdig.Parsers
{
    /// <summary>
    /// A List of <see cref="BlockParser"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.ParserList{Markdig.Parsers.BlockParser, Markdig.Parsers.BlockParserState}" />
    public class BlockParserList : ParserList<BlockParser, BlockProcessor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockParserList"/> class.
        /// </summary>
        /// <param name="parsers">The parsers.</param>
        public BlockParserList(IEnumerable<BlockParser> parsers) : base(parsers)
        {
        }
    }
}