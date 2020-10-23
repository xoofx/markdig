// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.CustomContainers
{
    /// <summary>
    /// A block custom container.
    /// </summary>
    /// <seealso cref="ContainerBlock" />
    /// <seealso cref="IFencedBlock" />
    public class CustomContainer : ContainerBlock, IFencedBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomContainer"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public CustomContainer(BlockParser parser) : base(parser)
        {
        }

        public char FencedChar { get; set; }

        public string WhitespaceAfterFencedChar { get; set; }

        public string Info { get; set; }

        public string WhitespaceAfterInfo { get; set; }

        public string Arguments { get; set; }
        public string WhitespaceAfterArguments { get; set; }

        public int OpeningFencedCharCount { get; set; }

        public int ClosingFencedCharCount { get; set; }

        public Newline InfoNewline { get; set; }
        public StringSlice WhitespaceBeforeClosingFence { get; set; }
    }
}