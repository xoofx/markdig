// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using System.Diagnostics;

namespace Markdig.Syntax.Inlines;

public enum LocalLabel : byte
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
    private TriviaProperties? _trivia => GetTrivia<TriviaProperties>();
    private TriviaProperties Trivia => GetOrSetTrivia<TriviaProperties>();

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
    public string? Label { get; set; }

    /// <summary>
    /// The label span
    /// </summary>
    public SourceSpan LabelSpan;

    /// <summary>
    /// Gets or sets the <see cref="Label"/> with trivia.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice LabelWithTrivia { get => _trivia?.LabelWithTrivia ?? StringSlice.Empty; set => Trivia.LabelWithTrivia = value; }

    /// <summary>
    /// Gets or sets the type of label parsed
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="LocalLabel.None"/>.
    /// </summary>
    public LocalLabel LocalLabel { get => _trivia?.LocalLabel ?? LocalLabel.None; set => Trivia.LocalLabel = value; }

    /// <summary>
    /// Gets or sets the reference this link is attached to. May be null.
    /// </summary>
    public LinkReferenceDefinition? Reference { get; set; }

    /// <summary>
    /// Gets or sets the label as matched against the <see cref="LinkReferenceDefinition"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled.
    /// </summary>
    public string? LinkRefDefLabel { get => _trivia?.LinkRefDefLabel; set => Trivia.LinkRefDefLabel = value; }

    /// <summary>
    /// Gets or sets the <see cref="LinkRefDefLabel"/> with trivia as matched against
    /// the <see cref="LinkReferenceDefinition"/>
    /// </summary>
    public StringSlice LinkRefDefLabelWithTrivia { get => _trivia?.LinkRefDefLabelWithTrivia ?? StringSlice.Empty; set => Trivia.LinkRefDefLabelWithTrivia = value; }

    /// <summary>
    /// Gets or sets the trivia before the <see cref="Url"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice TriviaBeforeUrl { get => _trivia?.TriviaBeforeUrl ?? StringSlice.Empty; set => Trivia.TriviaBeforeUrl = value; }

    /// <summary>
    /// True if the <see cref="Url"/> in the source document is enclosed
    /// in pointy brackets.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// false.
    /// </summary>
    public bool UrlHasPointyBrackets { get => _trivia?.UrlHasPointyBrackets ?? false; set => Trivia.UrlHasPointyBrackets = value; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The URL source span.
    /// </summary>
    public SourceSpan UrlSpan;

    /// <summary>
    /// The <see cref="Url"/> but with trivia and unescaped characters
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice UnescapedUrl { get => _trivia?.UnescapedUrl ?? StringSlice.Empty; set => Trivia.UnescapedUrl = value; }

    /// <summary>
    /// Any trivia after the <see cref="Url"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice TriviaAfterUrl { get => _trivia?.TriviaAfterUrl ?? StringSlice.Empty; set => Trivia.TriviaAfterUrl = value; }

    /// <summary>
    /// Gets or sets the GetDynamicUrl delegate. If this property is set, 
    /// it is used instead of <see cref="Url"/> to get the Url from this instance.
    /// </summary>
    public GetUrlDelegate? GetDynamicUrl { get; set; }

    /// <summary>
    /// Gets or sets the character used to enclose the <see cref="Title"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled.
    /// </summary>
    public char TitleEnclosingCharacter { get => _trivia?.TitleEnclosingCharacter ?? default; set => Trivia.TitleEnclosingCharacter = value; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The title source span.
    /// </summary>
    public SourceSpan TitleSpan;

    /// <summary>
    /// Gets or sets the <see cref="Title"/> exactly as parsed from the
    /// source document including unescaped characters
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice UnescapedTitle { get => _trivia?.UnescapedTitle ?? StringSlice.Empty; set => Trivia.UnescapedTitle = value; }

    /// <summary>
    /// Gets or sets the trivia after the <see cref="Title"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice TriviaAfterTitle { get => _trivia?.TriviaAfterTitle ?? StringSlice.Empty; set => Trivia.TriviaAfterTitle = value; }

    /// <summary>
    /// Gets or sets a boolean indicating if this link is a shortcut link to a <see cref="LinkReferenceDefinition"/>
    /// </summary>
    public bool IsShortcut { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating whether the inline link was parsed using markdown syntax or was automatic recognized.
    /// </summary>
    public bool IsAutoLink { get; set; }

    private sealed class TriviaProperties
    {
        public StringSlice LabelWithTrivia;
        public LocalLabel LocalLabel;
        public string? LinkRefDefLabel;
        public StringSlice LinkRefDefLabelWithTrivia;
        public StringSlice TriviaBeforeUrl;
        public bool UrlHasPointyBrackets;
        public StringSlice UnescapedUrl;
        public StringSlice TriviaAfterUrl;
        public char TitleEnclosingCharacter;
        public StringSlice UnescapedTitle;
        public StringSlice TriviaAfterTitle;
    }
}
