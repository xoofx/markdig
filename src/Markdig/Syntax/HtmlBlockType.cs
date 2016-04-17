// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Syntax
{
    /// <summary>
    /// Defines the type of <see cref="HtmlBlock"/>
    /// </summary>
    public enum HtmlBlockType
    {
        /// <summary>
        /// A SGML document type starting by &lt;!LETTER.
        /// </summary>
        DocumentType,

        /// <summary>
        /// A raw CDATA sequence.
        /// </summary>
        CData,

        /// <summary>
        /// A HTML comment.
        /// </summary>
        Comment,

        /// <summary>
        /// A SGM processing instruction tag &lt;?
        /// </summary>
        ProcessingInstruction,

        /// <summary>
        /// A script pre or style tag.
        /// </summary>
        ScriptPreOrStyle,

        /// <summary>
        /// An HTML interrupting block
        /// </summary>
        InterruptingBlock,

        /// <summary>
        /// An HTML non-interrupting block
        /// </summary>
        NonInterruptingBlock
    }
}