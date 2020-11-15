// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines
{
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
            OpeningCharacters = new[] {'[', ']', '!'};
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

            switch (c)
            {
                case '[':
                    // If this is not an image, we may have a reference link shortcut
                    // so we try to resolve it here
                    var saved = slice;

                    // If the label is followed by either a ( or a [, this is not a shortcut
                    if (LinkHelper.TryParseLabel(ref slice, out string label, out SourceSpan labelSpan))
                    {
                        if (!processor.Document.ContainsLinkReferenceDefinition(label))
                        {
                            label = null;
                        }
                    }
                    slice = saved;

                    // Else we insert a LinkDelimiter
                    slice.NextChar();
                    processor.Inline = new LinkDelimiterInline(this)
                    {
                        Type = DelimiterType.Open,
                        Label = label,
                        LabelSpan = processor.GetSourcePositionFromLocalSpan(labelSpan),
                        IsImage = isImage,
                        Span = new SourceSpan(startPosition, processor.GetSourcePosition(slice.Start - 1)),
                        Line = line,
                        Column = column
                    };
                    return true;

                case ']':
                    slice.NextChar();
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

        private bool ProcessLinkReference(InlineProcessor state, string label, bool isShortcut, SourceSpan labelSpan, LinkDelimiterInline parent, int endPosition)
        {
            if (!state.Document.TryGetLinkReferenceDefinition(label, out LinkReferenceDefinition linkRef))
            {
                return false;
            }

            Inline link = null;
            // Try to use a callback directly defined on the LinkReferenceDefinition
            if (linkRef.CreateLinkInline != null)
            {
                link = linkRef.CreateLinkInline(state, linkRef, parent.FirstChild);
            }

            // Create a default link if the callback was not found
            if (link == null)
            {
                // Inline Link
                link = new LinkInline()
                {
                    Url = HtmlHelper.Unescape(linkRef.Url),
                    Title = HtmlHelper.Unescape(linkRef.Title),
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
            }

            if (link is ContainerInline containerLink)
            {
                var child = parent.FirstChild;
                if (child == null)
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
            LinkDelimiterInline openParent = inlineState.Inline.FirstParentOfType<LinkDelimiterInline>();

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
                if (LinkHelper.TryParseInlineLink(ref text, out string url, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan))
                {
                    // Inline Link
                    var link = new LinkInline()
                    {
                        Url = HtmlHelper.Unescape(url),
                        Title = HtmlHelper.Unescape(title),
                        IsImage = openParent.IsImage,
                        LabelSpan = openParent.LabelSpan,
                        UrlSpan = inlineState.GetSourcePositionFromLocalSpan(linkSpan),
                        TitleSpan = inlineState.GetSourcePositionFromLocalSpan(titleSpan),
                        Span = new SourceSpan(openParent.Span.Start, inlineState.GetSourcePosition(text.Start - 1)),
                        Line = openParent.Line,
                        Column = openParent.Column,
                    };

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
            string label = null;
            bool isLabelSpanLocal = true;

            bool isShortcut = false;
            // Handle Collapsed links
            if (text.CurrentChar == '[')
            {
                if (text.PeekChar() == ']')
                {
                    label = openParent.Label;
                    labelSpan = openParent.LabelSpan;
                    isLabelSpanLocal = false;
                    text.NextChar(); // Skip [
                    text.NextChar(); // Skip ]
                }
            }
            else
            {
                label = openParent.Label;
                isShortcut = true;
            }

            if (label != null || LinkHelper.TryParseLabel(ref text, true, out label, out labelSpan))
            {
                if (isLabelSpanLocal)
                {
                    labelSpan = inlineState.GetSourcePositionFromLocalSpan(labelSpan);
                }

                if (ProcessLinkReference(inlineState, label, isShortcut, labelSpan, openParent, inlineState.GetSourcePosition(text.Start - 1)))
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
        }

        private void MarkParentAsInactive(Inline inline)
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
}