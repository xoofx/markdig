// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Syntax
{
    /// <summary>
    /// A common interface for fenced block (e.g: <see cref="FencedCodeBlock"/> or <see cref="Markdig.Extensions.CustomContainers.CustomContainer"/>)
    /// </summary>
    public interface IFencedBlock : IBlock
    {
        /// <summary>
        /// Gets or sets the language parsed after the first line of 
        /// the fenced code block. May be null.
        /// </summary>
        string Info { get; set; }

        /// <summary>
        /// Gets or sets the arguments after the <see cref="Info"/>.
        /// May be null.
        /// </summary>
        string Arguments { get; set; }

        /// <summary>
        /// Gets or sets the fenced character count used to open this fenced code block.
        /// </summary>
        int FencedCharCount { get; set; }

        /// <summary>
        /// Gets or sets the fenced character used to open and close this fenced code block.
        /// </summary>
        char FencedChar { get; set; }
    }
}