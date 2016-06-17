// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// A Link inline (Section 6.5 CommonMark specs)
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.ContainerInline" />
    [DebuggerDisplay("Url: {Url} Title: {Title} Image: {IsImage}")]
    public class LinkInline : ContainerInline
    {
        /// <summary>
        /// A delegate to use if it is setup on this instance to allow late binding 
        /// of a Url.
        /// </summary>
        /// <returns></returns>
        public delegate string GetUrlDelegate();

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkInline"/> class.
        /// </summary>
        public LinkInline()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkInline"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="title">The title.</param>
        public LinkInline(string url, string title)
        {
            Url = url;
            Title = title;
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the GetDynamicUrl delegate. If this property is set, 
        /// it is used instead of <see cref="Url"/> to get the Url from this instance.
        /// </summary>
        public GetUrlDelegate GetDynamicUrl { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is an image link.
        /// </summary>
        public bool IsImage { get; set; }

        /// <summary>
        /// Gets or sets the reference this link is attached to. May be null.
        /// </summary>
        public LinkReferenceDefinition Reference { get; set; }

        /// <summary>
        /// The URL source span.
        /// </summary>
        public SourceSpan? UrlSpan;

        /// <summary>
        /// The title source span.
        /// </summary>
        public SourceSpan? TitleSpan;

        /// <summary>
        /// The label span
        /// </summary>
        public SourceSpan? LabelSpan;
    }
}
