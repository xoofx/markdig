// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;

namespace Markdig.Syntax;

/// <summary>
/// A link reference definition (Section 4.7 CommonMark specs)
/// </summary>
/// <seealso cref="LeafBlock" />
public class LinkReferenceDefinition : LeafBlock
{
    private TriviaProperties? _trivia => TryGetDerivedTrivia<TriviaProperties>();
    private TriviaProperties Trivia => GetOrSetDerivedTrivia<TriviaProperties>();

    /// <summary>
    /// Creates an inline link for the specified <see cref="LinkReferenceDefinition"/>.
    /// </summary>
    /// <param name="inlineState">State of the inline.</param>
    /// <param name="linkRef">The link reference.</param>
    /// <param name="child">The child.</param>
    /// <returns>An inline link or null to use the default implementation</returns>
    public delegate Inline CreateLinkInlineDelegate(InlineProcessor inlineState, LinkReferenceDefinition linkRef, Inline? child = null);

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
    public LinkReferenceDefinition(string? label, string? url, string? title) : this()
    {
        Label = label;
        Url = url;
        Title = title;
    }

    /// <summary>
    /// Gets or sets the label. Text is normalized according to spec.
    /// </summary>
    /// https://spec.commonmark.org/0.29/#matches
    public string? Label { get; set; }

    /// <summary>
    /// The label span
    /// </summary>
    public SourceSpan LabelSpan;

    /// <summary>
    /// Non-normalized Label (includes trivia)
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice LabelWithTrivia { get => _trivia?.LabelWithTrivia ?? StringSlice.Empty; set => Trivia.LabelWithTrivia = value; }

    /// <summary>
    /// Whitespace before the <see cref="Url"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice TriviaBeforeUrl { get => _trivia?.TriviaBeforeUrl ?? StringSlice.Empty; set => Trivia.TriviaBeforeUrl = value; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The URL span
    /// </summary>
    public SourceSpan UrlSpan;

    /// <summary>
    /// Non-normalized <see cref="Url"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice UnescapedUrl { get => _trivia?.UnescapedUrl ?? StringSlice.Empty; set => Trivia.UnescapedUrl = value; }

    /// <summary>
    /// True when the <see cref="Url"/> is enclosed in point brackets in the source document.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// false.
    /// </summary>
    public bool UrlHasPointyBrackets { get => _trivia?.UrlHasPointyBrackets ?? false; set => Trivia.UrlHasPointyBrackets = value; }

    /// <summary>
    /// gets or sets the whitespace before a <see cref="Title"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice TriviaBeforeTitle { get => _trivia?.TriviaBeforeTitle ?? StringSlice.Empty; set => Trivia.TriviaBeforeTitle = value; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The title span
    /// </summary>
    public SourceSpan TitleSpan;

    /// <summary>
    /// Non-normalized <see cref="Title"/>.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
    /// <see cref="StringSlice.Empty"/>.
    /// </summary>
    public StringSlice UnescapedTitle { get => _trivia?.UnescapedTitle ?? StringSlice.Empty; set => Trivia.UnescapedTitle = value; }

    /// <summary>
    /// Gets or sets the character the <see cref="Title"/> is enclosed in.
    /// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise \0.
    /// </summary>
    public char TitleEnclosingCharacter { get => _trivia?.TitleEnclosingCharacter ?? default; set => Trivia.TitleEnclosingCharacter = value; }

    /// <summary>
    /// Gets or sets the create link inline callback for this instance.
    /// </summary>
    /// <remarks>
    /// This callback is called when an inline link is matching this reference definition.
    /// </remarks>
    public CreateLinkInlineDelegate? CreateLinkInline { get; set; }

    /// <summary>
    /// Tries to the parse the specified text into a definition.
    /// </summary>
    /// <typeparam name="T">Type of the text</typeparam>
    /// <param name="text">The text.</param>
    /// <param name="block">The block.</param>
    /// <returns><c>true</c> if parsing is successful; <c>false</c> otherwise</returns>
    public static bool TryParse<T>(ref T text, [NotNullWhen(true)] out LinkReferenceDefinition? block) where T : ICharIterator
    {
        block = null;

        var startSpan = text.Start;

        if (!LinkHelper.TryParseLinkReferenceDefinition(ref text, out string? label, out string? url, out string? title, out SourceSpan labelSpan, out SourceSpan urlSpan, out SourceSpan titleSpan))
        {
            return false;
        }

        block = new LinkReferenceDefinition(label, url, title)
        {
            LabelSpan = labelSpan,
            UrlSpan = urlSpan,
            TitleSpan = titleSpan,
            Span = new SourceSpan(startSpan, titleSpan.End > 0 ? titleSpan.End: urlSpan.End)
        };
        return true;
    }

    /// <summary>
    /// Tries to the parse the specified text into a definition.
    /// </summary>
    /// <typeparam name="T">Type of the text</typeparam>
    /// <param name="text">The text.</param>
    /// <param name="block">The block.</param>
    /// <param name="triviaBeforeLabel"></param>
    /// <param name="labelWithTrivia"></param>
    /// <param name="triviaBeforeUrl"></param>
    /// <param name="unescapedUrl"></param>
    /// <param name="triviaBeforeTitle"></param>
    /// <param name="unescapedTitle"></param>
    /// <param name="triviaAfterTitle"></param>
    /// <returns><c>true</c> if parsing is successful; <c>false</c> otherwise</returns>
    public static bool TryParseTrivia<T>(
        ref T text,
        [NotNullWhen(true)] out LinkReferenceDefinition? block,
        out SourceSpan triviaBeforeLabel,
        out SourceSpan labelWithTrivia,
        out SourceSpan triviaBeforeUrl,
        out SourceSpan unescapedUrl,
        out SourceSpan triviaBeforeTitle,
        out SourceSpan unescapedTitle,
        out SourceSpan triviaAfterTitle) where T : ICharIterator
    {
        block = null;

        var startSpan = text.Start;

        if (!LinkHelper.TryParseLinkReferenceDefinitionTrivia(
            ref text,
            out triviaBeforeLabel,
            out string? label,
            out labelWithTrivia,
            out triviaBeforeUrl,
            out string? url,
            out unescapedUrl,
            out bool urlHasPointyBrackets,
            out triviaBeforeTitle,
            out string? title,
            out unescapedTitle,
            out char titleEnclosingCharacter,
            out NewLine newLine,
            out triviaAfterTitle,
            out SourceSpan labelSpan,
            out SourceSpan urlSpan,
            out SourceSpan titleSpan))
        {
            return false;
        }

        block = new LinkReferenceDefinition(label, url, title)
        {
            UrlHasPointyBrackets = urlHasPointyBrackets,
            TitleEnclosingCharacter = titleEnclosingCharacter,
            //LabelWithWhitespace = labelWithWhitespace,
            LabelSpan = labelSpan,
            UrlSpan = urlSpan,
            //UnescapedUrl = unescapedUrl,
            //UnescapedTitle = unescapedTitle,
            TitleSpan = titleSpan,
            Span = new SourceSpan(startSpan, titleSpan.End > 0 ? titleSpan.End : urlSpan.End),
            NewLine = newLine,
        };
        return true;
    }

    private sealed class TriviaProperties
    {
        public StringSlice LabelWithTrivia;
        public StringSlice TriviaBeforeUrl;
        public StringSlice UnescapedUrl;
        public bool UrlHasPointyBrackets;
        public StringSlice TriviaBeforeTitle;
        public StringSlice UnescapedTitle;
        public char TitleEnclosingCharacter;
    }
}