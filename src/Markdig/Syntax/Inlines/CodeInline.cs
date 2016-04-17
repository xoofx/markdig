// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// Represents a code span (Section 6.3 CommonMark specs)
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.LeafInline" />
    [DebuggerDisplay("`{Content}`")]
    public class CodeInline : LeafInline
    {
        /// <summary>
        /// Gets or sets the delimiter character used by this code inline.
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the content of the span.
        /// </summary>
        public string Content { get; set; }
    }
}