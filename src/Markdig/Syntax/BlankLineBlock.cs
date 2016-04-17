// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Syntax
{
    /// <summary>
    /// A blank line, used internally by some parsers to store blank lines in a container. They are removed before the end of the document.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Block" />
    public sealed class BlankLineBlock : Block
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlankLineBlock"/> class.
        /// </summary>
        public BlankLineBlock() : base(null)
        {
            // A blankline is never opened
            IsOpen = false;
        }
    }
}