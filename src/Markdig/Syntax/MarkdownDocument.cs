// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Syntax
{
    /// <summary>
    /// The root Markdown document.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class MarkdownDocument : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownDocument"/> class.
        /// </summary>
        public MarkdownDocument() : base(null)
        {
        }
    }
}