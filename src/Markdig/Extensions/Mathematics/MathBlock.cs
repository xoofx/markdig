// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Mathematics
{
    /// <summary>
    /// A math block.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.FencedCodeBlock" />
    public class MathBlock : FencedCodeBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public MathBlock(BlockParser parser) : base(parser)
        {
        }
    }
}