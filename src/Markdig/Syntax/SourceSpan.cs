// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Syntax
{
    /// <summary>
    /// A span of text.
    /// </summary>
    public struct SourceSpan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceSpan"/> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public SourceSpan(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets or sets the starting character position from the original text source. 
        /// Note that for inline elements, this is only valid if <see cref="MarkdownExtensions.UsePreciseSourceLocation"/> is setup on the pipeline.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the ending character position from the original text source.
        /// Note that for inline elements, this is only valid if <see cref="MarkdownExtensions.UsePreciseSourceLocation"/> is setup on the pipeline.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets the character length of this element within the original source code.
        /// </summary>
        public int Length => End - Start + 1;
    }
}