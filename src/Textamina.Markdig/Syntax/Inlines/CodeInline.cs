// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Textamina.Markdig.Syntax.Inlines
{
    /// <summary>
    /// Represents a code span (Section 6.3 CommonMark specs)
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.Inlines.LeafInline" />
    public class CodeInline : LeafInline
    {
        /// <summary>
        /// Gets or sets the content of the span.
        /// </summary>
        public string Content { get; set; }
    }
}