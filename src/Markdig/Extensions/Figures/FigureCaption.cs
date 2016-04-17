// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Figures
{
    /// <summary>
    /// Defines a figure caption.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.LeafBlock" />
    public class FigureCaption : LeafBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FigureCaption"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public FigureCaption(BlockParser parser) : base(parser)
        {
            ProcessInlines = true;
        }
    }
}