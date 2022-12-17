// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// Represents the options used to create footnote html.
    /// </summary>
    public class FootnoteOptions
    {
        /// <summary>
        /// Initializes a new instance of the FootnoteOptions class.
        /// </summary>
        public FootnoteOptions()
        {
            this.CreateBackLinks = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the backlink should be added to the footnote.
        /// </summary>
        public bool CreateBackLinks { get; set; }

        /// <summary>
        /// Gets or sets the type of label to use for footnotes.
        /// </summary>
        public FootnoteLabelType LabelType { get; set; }

        /// <summary>
        /// Return either order or label based on the LabelType.
        /// </summary>
        /// <param name="order">The order in which the footnote appeared.</param>
        /// <param name="label">The original label of the footnote in markdown.</param>
        /// <returns>A string containg order if LabelType is NumberBasedOnOrder, otherwise label.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if LabelType is of an invalid type.</exception>
        internal string GetFootnoteLabel(string order, string label)
        {
            switch (LabelType)
            {
                case FootnoteLabelType.NumberBasedOnOrder:
                    return order;
                case FootnoteLabelType.PreserveMarkdownLabel:
                    // The label starts with ^ so remove that
                    return label.Substring(1);
                default:
                    throw new ArgumentOutOfRangeException("options");
            }
        }
    }
}