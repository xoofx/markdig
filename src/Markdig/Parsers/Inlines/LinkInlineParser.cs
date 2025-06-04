// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines;

/// <summary>
/// An inline parser for <see cref="LinkInline"/>.
/// </summary>
/// <seealso cref="InlineParser" />
public class LinkInlineParser : InlineParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkInlineParser"/> class.
    /// </summary>
    public LinkInlineParser()
    {
        OpeningCharacters = ['[', ']', '!'];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // The following methods are inspired by the "An algorithm for parsing nested emphasis and links"
        // at the end of the CommonMark specs.

        var c = slice.CurrentChar;

        var startPosition = processor.GetSourcePosition(slice.Start, out int line, out int column);

        bool isImage = false;
        if (c == '!')
        {
            isImage = true;
            c = slice.NextChar();
            if (c != '[')
            {
                return false;
            }
        }
        string? label;
        SourceSpan labelWithTriviaSpan = SourceSpan.Empty;
        switch (c)
        {
            case '[':
                // If this is not an image, we may have a reference link shortcut
                // so we try to resolve it here
                var saved = slice;

                SourceSpan labelSpan;
                // If the label is followed by either a ( or a [, this is not a shortcut
                if (processor.TrackTrivia)
                {
                    if (LinkHelper.TryParseLabelTrivia(ref slice, out label, out labelSpan))
                    {
                        labelWithTriviaSpan.Start = labelSpan.Start; // skip opening [
                        labelWithTriviaSpan.End = labelSpan.End; // skip closing ]
                        if (!processor.Document.ContainsLinkReferenceDefinition(label))
                        {
                            label = null;
                        }
                    }
                }
                else
                {
                    if (LinkHelper.TryParseLabel(ref slice, out label, out labelSpan))
                    {
                        if (!processor.Document.ContainsLinkReferenceDefinition(label))
                        {
                            label = null;
                        }
                    }
                }
                slice = saved;

                // Else we insert a LinkDelimiter
                slice.SkipChar();
                var linkDelimiter = new LinkDelimiterInline(this)
                {
                    Type = DelimiterType.Open,
                    Label = label,
                    LabelSpan = processor.GetSourcePositionFromLocalSpan(labelSpan),
                    IsImage = isImage,
                    Span = new SourceSpan(startPosition, processor.GetSourcePosition(slice.Start - 1)),
                    Line = line,
                    Column = column
                };

                if (processor.TrackTrivia)
                {
                    linkDelimiter.LabelWithTrivia = new StringSlice(slice.Text, labelWithTriviaSpan.Start, labelWithTriviaSpan.End);
                }

                processor.Inline = linkDelimiter;
                return true;

            case ']':
                slice.SkipChar();
                if (processor.Inline != null)
                {
                    if (TryProcessLinkOrImage(processor, ref slice))
                    {
                        return true;
                    }
                }

                // If we don’t find one, we return a literal slice node ].
                // (Done after by the LiteralInline parser)
                return false;
        }

        // We don't have an emphasis
        return false;
    }

    private bool ProcessLinkReference(
        InlineProcessor state,
        StringSlice text,
        string label,
        SourceSpan labelWithriviaSpan,
        bool isShortcut,
        SourceSpan labelSpan,
        LinkDelimiterInline parent,
        int endPosition,
        LocalLabel localLabel)
    {
        if (!state.Document.TryGetLinkReferenceDefinition(label, out LinkReferenceDefinition? linkRef))
        {
            return false;
        }

        Inline? link = null;
        // Try to use a callback directly defined on the LinkReferenceDefinition
        if (linkRef.CreateLinkInline != null)
        {
            link = linkRef.CreateLinkInline(state, linkRef, parent.FirstChild);
            link.Span = new SourceSpan(parent.Span.Start, endPosition);
            link.Line = parent.Line;
            link.Column = parent.Column;
        }

        // Create a default link if the callback was not found
        if (link is null)
        {
            // Inline Link
            var linkInline = new LinkInline()
            {
                Url = HtmlHelper.Unescape(linkRef.Url, removeBackSlash: false),
                Title = HtmlHelper.Unescape(linkRef.Title, removeBackSlash: false),
                Label = label,
                LabelSpan = labelSpan,
                UrlSpan = linkRef.UrlSpan,
                IsImage = parent.IsImage,
                IsShortcut = isShortcut,
                Reference = linkRef,
                Span = new SourceSpan(parent.Span.Start, endPosition),
                Line = parent.Line,
                Column = parent.Column,
            };

            if (state.TrackTrivia)
            {
                linkInline.LabelWithTrivia = new StringSlice(text.Text, labelWithriviaSpan.Start, labelWithriviaSpan.End);
                linkInline.LinkRefDefLabel = linkRef.Label;
                linkInline.LinkRefDefLabelWithTrivia = linkRef.LabelWithTrivia;
                linkInline.LocalLabel = localLabel;
            }

            link = linkInline;
        }

        if (link is ContainerInline containerLink)
        {
            var child = parent.FirstChild;
            if (child is null)
            {
                child = new LiteralInline()
                {
                    Content = StringSlice.Empty,
                    IsClosed = true,
                    // Not exact but we leave it like this
                    Span = parent.Span,
                    Line = parent.Line,
                    Column = parent.Column,
                };
                containerLink.AppendChild(child);
            }
            else
            {
                // Insert all child into the link
                while (child != null)
                {
                    var next = child.NextSibling;
                    child.Remove();
                    containerLink.AppendChild(child);
                    child = next;
                }
            }
        }

        link.IsClosed = true;

        // Process emphasis delimiters
        state.PostProcessInlines(0, link, null, false);

        state.Inline = link;

        return true;
    }

    private bool TryProcessLinkOrImage(InlineProcessor inlineState, ref StringSlice text)
    {
        LinkDelimiterInline? openParent = inlineState.Inline!.FirstParentOfType<LinkDelimiterInline>();

        if (openParent is null)
        {
            return false;
        }

        // If we do find one, but it’s not active,
        // we remove the inactive delimiter from the stack,
        // and return a literal text node ].
        if (!openParent.IsActive)
        {
            inlineState.Inline = new LiteralInline()
            {
                Content = new StringSlice("["),
                Span = openParent.Span,
                Line = openParent.Line,
                Column = openParent.Column,
            };
            openParent.ReplaceBy(inlineState.Inline);
            return false;
        }

        // If we find one and it’s active,
        // then we parse ahead to see if we have
        // an inline link/image, reference link/image,
        // compact reference link/image,
        // or shortcut reference link/image
        var parentDelimiter = openParent.Parent;
        var savedText = text;

        if (text.CurrentChar == '(')
        {
            LinkInline? link = null;

            if (inlineState.TrackTrivia)
            {
                link = TryParseInlineLinkTrivia(ref text, inlineState, openParent);
            }
            else
            {
                if (LinkHelper.TryParseInlineLink(ref text, out string? url, out string? title, out SourceSpan linkSpan, out SourceSpan titleSpan))
                {
                    // Inline Link
                    link = new LinkInline()
                    {
                        Url = HtmlHelper.Unescape(url, removeBackSlash: false),
                        Title = title is null ? null : HtmlHelper.Unescape(title, removeBackSlash: false),
                        IsImage = openParent.IsImage,
                        LabelSpan = openParent.LabelSpan,
                        UrlSpan = inlineState.GetSourcePositionFromLocalSpan(linkSpan),
                        TitleSpan = inlineState.GetSourcePositionFromLocalSpan(titleSpan),
                        Span = new SourceSpan(openParent.Span.Start, inlineState.GetSourcePosition(text.Start - 1)),
                        Line = openParent.Line,
                        Column = openParent.Column,
                    };
                }
            }

            if (link is not null)
            {
                openParent.ReplaceBy(link);
                // Notifies processor as we are creating an inline locally
                inlineState.Inline = link;

                // Process emphasis delimiters
                inlineState.PostProcessInlines(0, link, null, false);

                // If we have a link (and not an image),
                // we also set all [ delimiters before the opening delimiter to inactive.
                // (This will prevent us from getting links within links.)
                if (!openParent.IsImage)
                {
                    MarkParentAsInactive(parentDelimiter);
                }

                link.IsClosed = true;

                return true;
            }

            text = savedText;
        }

        var labelSpan = SourceSpan.Empty;
        string? label = null;
        bool isLabelSpanLocal = true;

        bool isShortcut = false;
        LocalLabel localLabel = LocalLabel.Local;
        // Handle Collapsed links
        if (text.CurrentChar == '[')
        {
            if (text.PeekChar() == ']')
            {
                label = openParent.Label;
                labelSpan = openParent.LabelSpan;
                isLabelSpanLocal = false;
                localLabel = LocalLabel.Empty;
                text.SkipChar(); // Skip [
                text.SkipChar(); // Skip ]
            }
        }
        else
        {
            localLabel = LocalLabel.None;
            label = openParent.Label;
            isShortcut = true;
        }

        if (label != null || LinkHelper.TryParseLabelTrivia(ref text, true, out label, out labelSpan))
        {
            var labelWithTrivia = new SourceSpan(labelSpan.Start, labelSpan.End);
            if (isLabelSpanLocal)
            {
                labelSpan = inlineState.GetSourcePositionFromLocalSpan(labelSpan);
            }

            if (ProcessLinkReference(inlineState, text, label!, labelWithTrivia, isShortcut, labelSpan, openParent, inlineState.GetSourcePosition(text.Start - 1), localLabel))
            {
                // Remove the open parent
                openParent.Remove();
                if (!openParent.IsImage)
                {
                    MarkParentAsInactive(parentDelimiter);
                }
                return true;
            }
            else if (text.CurrentChar != ']' && text.CurrentChar != '[')
            {
                return false;
            }
        }

        // We have a nested [ ]
        // firstParent.Remove();
        // The opening [ will be transformed to a literal followed by all the children of the [

        var literal = new LiteralInline()
        {
            Span = openParent.Span,
            Content = new StringSlice(openParent.IsImage ? "![" : "[")
        };

        inlineState.Inline = openParent.ReplaceBy(literal);
        return false;

        static LinkInline? TryParseInlineLinkTrivia(ref StringSlice text, InlineProcessor inlineState, LinkDelimiterInline openParent)
        {
            if (LinkHelper.TryParseInlineLinkTrivia(
                ref text,
                out string? url,
                out SourceSpan unescapedUrlSpan,
                out string? title,
                out SourceSpan unescapedTitleSpan,
                out char titleEnclosingCharacter,
                out SourceSpan linkSpan,
                out SourceSpan titleSpan,
                out SourceSpan triviaBeforeLink,
                out SourceSpan triviaAfterLink,
                out SourceSpan triviaAfterTitle,
                out bool urlHasPointyBrackets))
            {
                var wsBeforeLink = new StringSlice(text.Text, triviaBeforeLink.Start, triviaBeforeLink.End);
                var wsAfterLink = new StringSlice(text.Text, triviaAfterLink.Start, triviaAfterLink.End);
                var wsAfterTitle = new StringSlice(text.Text, triviaAfterTitle.Start, triviaAfterTitle.End);
                var unescapedUrl = new StringSlice(text.Text, unescapedUrlSpan.Start, unescapedUrlSpan.End);
                var unescapedTitle = new StringSlice(text.Text, unescapedTitleSpan.Start, unescapedTitleSpan.End);

                return new LinkInline()
                {
                    TriviaBeforeUrl = wsBeforeLink,
                    Url = HtmlHelper.Unescape(url, removeBackSlash: false),
                    UnescapedUrl = unescapedUrl,
                    UrlHasPointyBrackets = urlHasPointyBrackets,
                    TriviaAfterUrl = wsAfterLink,
                    Title = HtmlHelper.Unescape(title, removeBackSlash: false),
                    UnescapedTitle = unescapedTitle,
                    TitleEnclosingCharacter = titleEnclosingCharacter,
                    TriviaAfterTitle = wsAfterTitle,
                    IsImage = openParent.IsImage,
                    LabelSpan = openParent.LabelSpan,
                    UrlSpan = inlineState.GetSourcePositionFromLocalSpan(linkSpan),
                    TitleSpan = inlineState.GetSourcePositionFromLocalSpan(titleSpan),
                    Span = new SourceSpan(openParent.Span.Start, inlineState.GetSourcePosition(text.Start - 1)),
                    Line = openParent.Line,
                    Column = openParent.Column,
                };
            }

            return null;
        }
    }

    private static void MarkParentAsInactive(Inline? inline)
    {
        while (inline != null)
        {
            if (inline is LinkDelimiterInline linkInline)
            {
                if (linkInline.IsImage)
                {
                    break;
                }

                linkInline.IsActive = false;
            }

            inline = inline.Parent;
        }
    }
}