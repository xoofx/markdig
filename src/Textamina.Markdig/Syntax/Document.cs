// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// The root Markdown document.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.ContainerBlock" />
    public class Document : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document() : base(null)
        {
        }
    }
}