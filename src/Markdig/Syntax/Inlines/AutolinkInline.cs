// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// An autolink (Section 6.7 CommonMark specs)
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.LeafInline" />
    [DebuggerDisplay("<{Url}>")]
    public class AutolinkInline : LeafInline
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is an email link.
        /// </summary>
        public bool IsEmail { get; set; }

        /// <summary>
        /// Gets or sets the URL of this link.
        /// </summary>
        public string Url { get; set; }

        public override string ToString()
        {
            return Url;
        }
    }
}