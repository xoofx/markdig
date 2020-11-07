// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using System.Diagnostics;

namespace Markdig.Syntax.Inlines
{
    public enum LocalLabel
    {
        Local, // [foo][bar]
        Empty, // [foo][]
        None, // [foo]
    }
    /// <summary>
    /// A Link inline (Section 6.5 CommonMark specs)
    /// </summary>
    /// <seealso cref="ContainerInline" />
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
        /// Gets or sets a value indicating whether this instance is an image link.
        /// </summary>
        public bool IsImage { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The label span
        /// </summary>
        public SourceSpan? LabelSpan;

        /// <summary>
        /// Gets or sets the <see cref="Label"/> with whitespace.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice LabelWithWhitespace { get; set; }

        /// <summary>
        /// Gets or sets the type of label parsed
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="LocalLabel.None"/>.
        /// </summary>
        public LocalLabel LocalLabel { get; set; }

        /// <summary>
        /// Gets or sets the reference this link is attached to. May be null.
        /// </summary>
        public LinkReferenceDefinition Reference { get; set; }

        /// <summary>
        /// Gets or sets the label as matched against the <see cref="LinkReferenceDefinition"/>.
        /// </summary>
        public string LinkRefDefLabel { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LinkRefDefLabel"/> with whitespace as matched against
        /// the <see cref="LinkReferenceDefinition"/>
        /// </summary>
        public StringSlice LinkRefDefLabelWithWhitespace { get; set; }

        /// <summary>
        /// Gets or sets the whitespace before the <see cref="Url"/>.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice WhitespaceBeforeUrl { get; set; }

        /// <summary>
        /// True if the <see cref="Url"/> in the source document is enclosed
        /// in pointy brackets.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// false.
        /// </summary>
        public bool UrlHasPointyBrackets { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The URL source span.
        /// </summary>
        public SourceSpan? UrlSpan;

        /// <summary>
        /// The <see cref="Url"/> but with whitespace and unescaped characters
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice UnescapedUrl { get; set; }

        /// <summary>
        /// Any whitespace after the <see cref="Url"/>.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice WhitespaceAfterUrl { get; set; }

        /// <summary>
        /// Gets or sets the GetDynamicUrl delegate. If this property is set, 
        /// it is used instead of <see cref="Url"/> to get the Url from this instance.
        /// </summary>
        public GetUrlDelegate GetDynamicUrl { get; set; }

        /// <summary>
        /// Gets or sets the character used to enclose the <see cref="Title"/>.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public char TitleEnclosingCharacter { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The title source span.
        /// </summary>
        public SourceSpan? TitleSpan;

        /// <summary>
        /// Gets or sets the <see cref="Title"/> exactly as parsed from the
        /// source document including unescaped characters
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice UnescapedTitle { get; set; }

        /// <summary>
        /// Gets or sets the whitespace after the <see cref="Title"/>.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice WhitespaceAfterTitle { get; set; }

        /// <summary>
        /// Gets or sets a boolean indicating if this link is a shortcut link to a <see cref="LinkReferenceDefinition"/>
        /// </summary>
        public bool IsShortcut { get; set; }

        /// <summary>
        /// Gets or sets a boolean indicating whether the inline link was parsed using markdown syntax or was automatic recognized.
        /// </summary>
        public bool IsAutoLink { get; set; }
    }
}
