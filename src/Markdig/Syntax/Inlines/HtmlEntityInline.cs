// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Diagnostics;
using Markdig.Helpers;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// An entity HTML.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.LeafInline" />
    [DebuggerDisplay("{Original} -> {Transcoded}")]
    public class HtmlEntityInline : LeafInline
    {
        /// <summary>
        /// Gets or sets the original HTML entity name
        /// </summary>
        public StringSlice Original { get; set; }

        /// <summary>
        /// Gets or sets the transcoded literal that will be used for output
        /// </summary>
        public StringSlice Transcoded { get; set; }
    }
}