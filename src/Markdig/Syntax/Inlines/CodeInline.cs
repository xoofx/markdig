// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using System.Diagnostics;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// Represents a code span (Section 6.3 CommonMark specs)
    /// </summary>
    /// <seealso cref="LeafInline" />
    [DebuggerDisplay("`{Content}`")]
    public class CodeInline : LeafInline
    {
        public CodeInline(string content)
        {
            Content = content;
        }

        /// <summary>
        /// Gets or sets the delimiter character used by this code inline.
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the amount of delimiter characters used
        /// </summary>
        public int DelimiterCount { get; set; }

        /// <summary>
        /// Gets or sets the content of the span.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the content with trivia and whitespace.
        /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice ContentWithTrivia { get; set; }

        /// <summary>
        /// True if the first and last character of the content enclosed in a backtick `
        /// is a space.
        /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
        /// false.
        /// </summary>
        public bool FirstAndLastWasSpace { get; set; }
    }
}