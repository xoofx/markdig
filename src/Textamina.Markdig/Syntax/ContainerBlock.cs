// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using System.Diagnostics;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// A base class for container blocks.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.Block" />
    [DebuggerDisplay("{GetType().Name} Count = {Children.Count}")]
    public abstract class ContainerBlock : Block
    {
        // TODO: Remove Children and use only inner array

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        protected ContainerBlock(BlockParser parser) : base(parser)
        {
            Children = new List<Block>();
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public List<Block> Children { get; }

        /// <summary>
        /// Gets the last child.
        /// </summary>
        public Block LastChild => Children.Count > 0 ? Children[Children.Count - 1] : null;
    }
}