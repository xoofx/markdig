// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Repressents an indented code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.4 Indented code blocks 
    /// </remarks>
    public class CodeBlock : LeafBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public CodeBlock(BlockParser parser) : base(parser)
        {
        }
    }
}