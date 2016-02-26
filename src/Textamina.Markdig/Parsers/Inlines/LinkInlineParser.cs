using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class LinkInlineParser : InlineParser
    {
        public static readonly LinkInlineParser Default = new LinkInlineParser();

        public LinkInlineParser()
        {
            OpeningCharacters = new[] {'[', ']', '!'};
        }

        public override bool Match(InlineParserState state, ref StringSlice text)
        {
            var c = text.CurrentChar;

            bool isImage = false;
            if (c == '!')
            {
                isImage = true;
                c = text.NextChar();
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
                    var saved = text;
                    string label;

                    // If the label is followed by either a ( or a [, this is not a shortcut
                    if (LinkHelper.TryParseLabel(ref text, out label))
                    {
                        if (!state.Document.LinkReferenceDefinitions.ContainsKey(label))
                        {
                            label = null;
                        }
                    }
                    text = saved;

                    // Else we insert a LinkDelimiter
                    text.NextChar();
                    state.Inline = new LinkDelimiterInline(this)
                    {
                        Type = DelimiterType.Open,
                        Label = label,
                        IsImage = isImage
                    };
                    return true;

                case ']':
                    text.NextChar();
                    if (state.Inline != null)
                    {
                        if (TryProcessLinkOrImage(state, ref text))
                        {
                            return true;
                        }
                    }

                    // If we don’t find one, we return a literal text node ].
                    // (Done after by the LiteralInline parser)
                    return false;
            }

            // We don't have an emphasis
            return false;
        }

        private bool ProcessLinkReference(InlineParserState state, string label, bool isImage, Inline child = null)
        {
            bool isValidLink = false;
            LinkReferenceDefinitionBlock linkRef;
            if (state.Document.LinkReferenceDefinitions.TryGetValue(label, out linkRef))
            {
                // Inline Link
                var link = new LinkInline()
                {
                    Url = HtmlHelper.Unescape(linkRef.Url),
                    Title = HtmlHelper.Unescape(linkRef.Title),
                    IsImage = isImage,
                };

                if (child == null)
                {
                    child = new LiteralInline()
                    {
                        Content = label,
                        IsClosed = true
                    };
                    link.AppendChild(child);
                }
                else
                {
                    // Insert all child into the link
                    while (child != null)
                    {
                        var next = child.NextSibling;
                        child.Remove();
                        link.AppendChild(child);
                        child = next;
                    }
                }
                link.IsClosed = true;

                EmphasisInline.ProcessEmphasis(link);

                state.Inline = link;
                isValidLink = true;
            }
            //else
            //{
            //    // Else output a literal, leave it opened as we may have literals after
            //    // that could be append to this one
            //    var literal = new LiteralInline()
            //    {
            //        ContentBuilder = state.StringBuilders.Get().Append('[').Append(label).Append(']')
            //    };
            //    state.Inline = literal;
            //}
            return isValidLink;
        }

        private bool TryProcessLinkOrImage(InlineParserState inlineState, ref StringSlice text)
        {
            LinkDelimiterInline openParent = null;
            foreach (var parent in inlineState.Inline.FindParentOfType<LinkDelimiterInline>())
            {
                openParent = parent;
                break;
            }

            // This will be matched as a literal
            if (openParent != null)
            {
                var parentDelimiter = openParent.Parent;
                switch (text.CurrentChar)
                {
                    case '(':
                        string url;
                        string title;
                        if (LinkHelper.TryParseInlineLink(ref text, out url, out title))
                        {
                            // Inline Link
                            var link = new LinkInline()
                            {
                                Url = HtmlHelper.Unescape(url),
                                Title = HtmlHelper.Unescape(title),
                                IsImage = openParent.IsImage,
                            };

                            openParent.ReplaceBy(link);
                            inlineState.Inline = link;

                            EmphasisInline.ProcessEmphasis(link);

                            ReplaceParentIfNotImage(openParent.IsImage, parentDelimiter);

                            link.IsClosed = true;

                            return true;
                        }
                        break;
                    default:

                        string label = null;
                        // Handle Collapsed links
                        if (text.CurrentChar == '[')
                        {
                            if (text.PeekChar(1) == ']')
                            {
                                label = openParent.Label;
                                text.NextChar(); // Skip [
                                text.NextChar(); // Skip ]
                            }
                        }
                        else
                        {
                            label = openParent.Label;
                        }

                        if (label != null || LinkHelper.TryParseLabel(ref text, true, out label))
                        {
                            if (ProcessLinkReference(inlineState, label, openParent.IsImage,
                                openParent.FirstChild))
                            {
                                // Remove the open parent
                                openParent.Remove();
                                ReplaceParentIfNotImage(openParent.IsImage, parentDelimiter);
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
                    Content = openParent.IsImage ? "![" : "["
                };

                inlineState.InlinesToClose.Add(literal);
                inlineState.Inline = openParent.ReplaceBy(literal);
                return false;
            }

            return false;
        }

        private void ReplaceParentIfNotImage(bool isImage, Inline inline)
        {
            if (isImage || inline == null)
            {
                return;
            }

            foreach (var parent in inline.FindParentOfType<LinkDelimiterInline>())
            {
                if (parent.IsImage)
                {
                    break;
                }

                var literal = new LiteralInline()
                {
                    Content = "[",
                    IsClosed = true
                };

                parent.ReplaceBy(literal);
            }
        }

        private bool TryParseLinkTitle(InlineParserState state)
        {
            return false;
        }
    }
}