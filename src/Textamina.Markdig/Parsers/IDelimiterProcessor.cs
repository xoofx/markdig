// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// A procesor used for <see cref="DelimiterInline"/>.
    /// </summary>
    public interface IDelimiterProcessor
    {
        /// <summary>
        /// Processes the delimiters.
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="root">The root inline.</param>
        /// <param name="lastChild">The last child.</param>
        /// <param name="delimiterProcessorIndex">Index of this delimiter processor.</param>
        /// <param name="isFinalProcessing"></param>
        /// <returns><c>true</c> to continue to the next delimiter processor; 
        /// <c>false</c> to stop the process (in case a processor is perfoming sub-sequent processor itself)</returns>
        bool ProcessDelimiters(InlineProcessor state, Inline root, Inline lastChild, int delimiterProcessorIndex, bool isFinalProcessing);
    }
}