// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// A link reference definition (Section 4.7 CommonMark specs)
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.LeafBlock" />
    public class LinkReferenceDefinition : LeafBlock
    {
        /// <summary>
        /// TODO: document this.
        /// </summary>
        public static readonly object DocumentKey = typeof (LinkReferenceDefinition);

        /// <summary>
        /// Creates an inline link for the specified <see cref="LinkReferenceDefinition"/>.
        /// </summary>
        /// <param name="inlineState">State of the inline.</param>
        /// <param name="linkRef">The link reference.</param>
        /// <param name="child">The child.</param>
        /// <returns>An inline link or null to use the default implementation</returns>
        public delegate Inline CreateLinkInlineDelegate(InlineParserState inlineState, LinkReferenceDefinition linkRef, Inline child = null);

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkReferenceDefinition"/> class.
        /// </summary>
        public LinkReferenceDefinition() : base(null)
        {
            IsOpen = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkReferenceDefinition"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="url">The URL.</param>
        /// <param name="title">The title.</param>
        public LinkReferenceDefinition(string label, string url, string title) : this()
        {
            Label = label;
            Url = url;
            Title = title;
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the create link inline calback for this instance.
        /// </summary>
        /// <remarks>
        /// This callback is called when an inline link is matching this reference definition.
        /// </remarks>
        public CreateLinkInlineDelegate CreateLinkInline { get; set; }

        /// <summary>
        /// Tries to the parse the specified text into a definition.
        /// </summary>
        /// <typeparam name="T">Type of the text</typeparam>
        /// <param name="text">The text.</param>
        /// <param name="block">The block.</param>
        /// <returns><c>true</c> if parsing is successfull; <c>false</c> otherwise</returns>
        public static bool TryParse<T>(ref T text, out LinkReferenceDefinition block) where T : ICharIterator
        {
            block = null;
            string label;
            string url;
            string title;

            if (!LinkHelper.TryParseLinkReferenceDefinition(ref text, out label, out url, out title))
            {
                return false;
            }

            block = new LinkReferenceDefinition(label, url, title);
            return true;
        }
    }
}