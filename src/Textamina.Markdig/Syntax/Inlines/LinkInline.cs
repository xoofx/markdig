// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Syntax.Inlines
{
    /// <summary>
    /// A Link inline (Section 6.5 CommonMark specs)
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.Inlines.ContainerInline" />
    public class LinkInline : ContainerInline
    {
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
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is an image link.
        /// </summary>
        public bool IsImage { get; set; }

        public override string ToString()
        {
            return (IsImage ? "<img src=\"" : "<a href=\"") + Url + "\" title=\"" + Title + "\">";
        }
    }
}
