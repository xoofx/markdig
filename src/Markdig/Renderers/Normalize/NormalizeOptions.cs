// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Renderers.Normalize
{
    /// <summary>
    /// Defines the options used by <see cref="NormalizeRenderer"/>
    /// </summary>
    public class NormalizeOptions
    {
        /// <summary>
        /// Initialize a new instance of <see cref="NormalizeOptions"/>
        /// </summary>
        public NormalizeOptions()
        {
            SpaceAfterQuoteBlock = true;
            EmptyLineAfterCodeBlock = true;
            EmptyLineAfterHeading = true;
            EmptyLineAfterThematicBreak = true;
            ExpandAutoLinks = true;
            ListItemCharacter = null;
        }

        /// <summary>
        /// Adds a space after a QuoteBlock &gt;. Default is <c>true</c>
        /// </summary>
        public bool SpaceAfterQuoteBlock { get; set; }

        /// <summary>
        /// Adds an empty line after a code block (fenced and tabbed). Default is <c>true</c>
        /// </summary>
        public bool EmptyLineAfterCodeBlock { get; set; }

        /// <summary>
        /// Adds an empty line after an heading. Default is <c>true</c>
        /// </summary>
        public bool EmptyLineAfterHeading { get; set; }

        /// <summary>
        /// Adds an empty line after an thematic break. Default is <c>true</c>
        /// </summary>
        public bool EmptyLineAfterThematicBreak { get; set; }

        /// <summary>
        /// The bullet character used for list items. Default is <c>null</c> leaving the original bullet character as-is.
        /// </summary>
        public char? ListItemCharacter { get; set; }

        /// <summary>
        /// Expands AutoLinks to the normal inline representation. Default is <c>true</c>
        /// </summary>
        public bool ExpandAutoLinks { get; set; }
    }
}