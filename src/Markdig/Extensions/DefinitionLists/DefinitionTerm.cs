// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.DefinitionLists
{
    /// <summary>
    /// A definition term contains a single line with the term to define.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.LeafBlock" />
    public class DefinitionTerm : LeafBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionTerm"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public DefinitionTerm(BlockParser parser) : base(parser)
        {
            ProcessInlines = true;
        }
    }
}