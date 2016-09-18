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
    /// <seealso cref="Markdig.Parsers.InlineParser" />
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

            int line;
            int column;
            var startPosition = processor.GetSourcePosition(slice.Start, out line, out column);

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
                    string label;

                    SourceSpan labelSpan;
                    // If the label is followed by either a ( or a [, this is not a shortcut
                    if (LinkHelper.TryParseLabel(ref slice, out label, out labelSpan))
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

        private bool ProcessLinkReference(InlineProcessor state, string label, SourceSpan labelSpan, LinkDelimiterInline parent, int endPosition)
        {
            bool isValidLink = false;
            LinkReferenceDefinition linkRef;
            if (state.Document.TryGetLinkReferenceDefinition(label, out linkRef))
            {
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
                        Reference = linkRef,
                        Span = new SourceSpan(parent.Span.Start, endPosition),
                        Line = parent.Line,
                        Column = parent.Column,
                    };
                }

                var containerLink = link as ContainerInline;
                if (containerLink != null)
                {
                    var child = parent.FirstChild;
                    if (child == null)
                    {
                        child = new LiteralInline()
                        {
                            Content = new StringSlice(label),
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
                isValidLink = true;
            }
            //else
            //{
            //    // Else output a literal, leave it opened as we may have literals after
            //    // that could be append to this one
            //    var literal = new LiteralInline()
            //    {
            //        ContentBuilder = processor.StringBuilders.Get().Append('[').Append(label).Append(']')
            //    };
            //    processor.Inline = literal;
            //}
            return isValidLink;
        }

        private bool TryProcessLinkOrImage(InlineProcessor inlineState, ref StringSlice text)
        {
            LinkDelimiterInline openParent = null;
            foreach (var parent in inlineState.Inline.FindParentOfType<LinkDelimiterInline>())
            {
                openParent = parent;
                break;
            }

            if (openParent != null)
            {
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
                switch (text.CurrentChar)
                {
                    case '(':
                        string url;
                        string title;
                        SourceSpan linkSpan;
                        SourceSpan titleSpan;
                        if (LinkHelper.TryParseInlineLink(ref text, out url, out title, out linkSpan, out titleSpan))
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
                                Span = new SourceSpan(openParent.Span.Start, inlineState.GetSourcePosition(text.Start -1)),
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
                        goto default;
                    default:
                        var labelSpan = SourceSpan.Empty;
                        string label = null;
                        bool isLabelSpanLocal = true;
                        // Handle Collapsed links
                        if (text.CurrentChar == '[')
                        {
                            if (text.PeekChar(1) == ']')
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
                        }

                        if (label != null || LinkHelper.TryParseLabel(ref text, true, out label, out labelSpan))
                        {
                            if (isLabelSpanLocal)
                            {
                                labelSpan = inlineState.GetSourcePositionFromLocalSpan(labelSpan);
                            }

                            if (ProcessLinkReference(inlineState, label, labelSpan, openParent, inlineState.GetSourcePosition(text.Start - 1)))
                            {
                                // Remove the open parent
                                openParent.Remove();
                                if (!openParent.IsImage)
                                {
                                    MarkParentAsInactive(parentDelimiter);
                                }
                            }
                            else
                            {
                                return false;
                            }
                            return true;
                        }
                        break;
                }

                // We have a nested [ ]
                // firstParent.Remove();
                // The opening [ will be transformed to a literal followed by all the childrens of the [ 

                var literal = new LiteralInline()
                {
                    Span = openParent.Span,
                    Content = new StringSlice(openParent.IsImage ? "![" : "[")
                };

                inlineState.Inline = openParent.ReplaceBy(literal);
                return false;
            }

            return false;
        }

        private void MarkParentAsInactive(Inline inline)
        {
            if (inline == null)
            {
                return;
            }

            foreach (var parent in inline.FindParentOfType<LinkDelimiterInline>())
            {
                if (parent.IsImage)
                {
                    break;
                }

                parent.IsActive = false;
            }
        }
    }
}