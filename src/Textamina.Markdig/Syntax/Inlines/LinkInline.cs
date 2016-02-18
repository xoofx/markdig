using System;
using System.Text;
using Textamina.Markdig.Formatters;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class LinkInline : ContainerInline
    {
        public static readonly InlineParser Parser = new ParserInternal();


        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] {'[', ']', '!'};
            }

            public override bool Match(MatchInlineState state)
            {
                var text = state.Lines;

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

                if (c == '[')
                {
                    text.NextChar();
                    state.Inline = new LinkDelimiterInline(this)
                    {
                        Type = DelimiterType.Open,
                        IsImage = isImage
                    };
                    return true;
                }

                if (c == ']')
                {
                    text.NextChar();
                    if (state.Inline != null)
                    {
                        return TryParseEndOfLinkOrImage(ref state.Inline, text);
                    }
                    return false;
                    // Match a close tag
                }

                // We don't have an emphasis
                return false;
            }

            private bool TryParseEndOfLinkOrImage(ref Inline current, StringLineGroup text)
            {
                LinkDelimiterInline firstParent = null;
                foreach (var parent in current.FindParentOfType<LinkDelimiterInline>())
                {
                    firstParent = parent;
                    break;
                }

                // This will be matched as a literal
                if (firstParent != null)
                {
                    // TODO: continue parsing of ]

                    var savePoint = text.Save();

                    var url = string.Empty;
                    var title = string.Empty;
                    if (text.CurrentChar == '(')
                    {
                        if (LinkHelper.TryParseUrlAndTitle(text, out url, out title))
                        {
                            // Inline Link
                            var link = new TextLinkInline()
                            {
                                Url = HtmlHelper.Unescape(url),
                                Title = HtmlHelper.Unescape(title),
                                IsClosed = true,                                
                            };
                            firstParent.ReplaceBy(link);
                            current = link;
                            return true;
                        }
                    }

                    // We have a nested [ ]
                    // firstParent.Remove();
                    // The opening [ will be transformed to a literal followed by all the childrens of the [ 

                    var literal = new LiteralInline() { Content = firstParent.IsImage ? "![" : "[" };
                    current = firstParent.ReplaceBy(literal);
                    return false;
                }

                return false;
            }

            private bool TryParseLinkTitle(MatchInlineState state)
            {
                return false;
            }

        }
    }
}