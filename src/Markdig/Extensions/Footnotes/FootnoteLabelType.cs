// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// Represents the type of label used in footnotes.
    /// </summary>
    public enum FootnoteLabelType
    {
        /// <summary>
        /// An increasing number is used based on the order in which the footnotes appear.
        /// </summary>
        NumberBasedOnOrder = 0,

        /// <summary>
        /// The label used in markdown for the footnote is used as its reference.
        /// </summary>
        PreserveMarkdownLabel = 1
    }
}