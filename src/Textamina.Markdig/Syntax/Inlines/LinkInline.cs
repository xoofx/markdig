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

        public static bool TryParseUrlAndTitle(StringLineGroup text, out string link, out string title)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            // 1. An inline link consists of a link text followed immediately by a left parenthesis (, 
            // 2. optional whitespace,  TODO: specs: is it whitespace or multiple whitespaces?
            // 3. an optional link destination, 
            // 4. an optional link title separated from the link destination by whitespace, 
            // 5. optional whitespace,  TODO: specs: is it whitespace or multiple whitespaces?
            // 6. and a right parenthesis )
            bool isValid = false;
            var c = text.Current;
            link = null;
            title = null;

            // 1. An inline link consists of a link text followed immediately by a left parenthesis (, 
            if (c == '(')
            {
                text.NextChar();
                text.SkipWhiteSpaces();

                if (TryParseLink(text, out link))
                {
                    var hasWhiteSpaces = text.SkipWhiteSpaces();

                    c = text.Current;
                    if (c == ')')
                    {
                        isValid = true;
                    }
                    else if (hasWhiteSpaces)
                    {
                        c = text.Current;
                        if (c == ')')
                        {
                            isValid = true;
                        }
                        else if (TryParseTitle(text, out title))
                        {
                            text.SkipWhiteSpaces();
                            c = text.Current;

                            if (c == ')')
                            {
                                isValid = true;
                            }
                        }
                    }
                }
            }

            if (isValid)
            {
                // Skip ')'
                text.NextChar();
                title = title ?? string.Empty;
            }

            return isValid;
        }

        public static bool TryParseTitle(StringLineGroup text, out string title)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            bool isValid = false;
            var buffer = StringBuilderCache.Local();
            buffer.Clear();

            // a sequence of zero or more characters between straight double-quote characters ("), including a " character only if it is backslash-escaped, or
            // a sequence of zero or more characters between straight single-quote characters ('), including a ' character only if it is backslash-escaped, or
            var c = text.Current;
            if (c == '\'' || c == '"')
            {
                var quote = c;
                bool hasEscape = false;

                while (true)
                {
                    c = text.NextChar();

                    if (c == '\0')
                    {
                        break;
                    }

                    if (c == quote)
                    {
                        if (hasEscape)
                        {
                            buffer.Append(quote);
                            hasEscape = false;
                            continue;
                        }

                        // Skip last quote
                        text.NextChar();
                        isValid = true;
                        break;
                    }

                    if (hasEscape && !CharHelper.IsAsciiPunctuation(c))
                    {
                        buffer.Append('\\');
                    }

                    if (c == '\\')
                    {
                        hasEscape = true;
                        continue;
                    }

                    hasEscape = false;

                    buffer.Append(c);
                }
            }
            else
            {
                // a sequence of zero or more characters between matching parentheses ((...)), including a ) character only if it is backslash-escaped.
                // TODO

                isValid = true;
            }

            title = isValid ? buffer.ToString() : null;
            buffer.Clear();
            return isValid;
        }


        public static bool TryParseLink(StringLineGroup text, out string link)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            bool isValid = false;
            var buffer = StringBuilderCache.Local();
            buffer.Clear();

            var c = text.Current;

            // a sequence of zero or more characters between an opening < and a closing > 
            // that contains no spaces, line breaks, or unescaped < or > characters, or
            if (c == '<')
            {
                bool hasEscape = false;
                do
                {
                    c = text.NextChar();
                    if (!hasEscape && c == '>')
                    {
                        text.NextChar();
                        isValid = true;
                        break;
                    }

                    if (!hasEscape && c == '<')
                    {
                        break;
                    }

                    if (hasEscape && !CharHelper.IsAsciiPunctuation(c))
                    {
                        buffer.Append('\\');
                    }

                    if (c == '\\')
                    {
                        hasEscape = true;
                        continue;
                    }

                    hasEscape = false;

                    if (CharHelper.IsWhitespace(c)) // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
                    {
                        break;
                    }

                    buffer.Append(c);

                } while (c != '\0');
            }
            else
            {
                // a nonempty sequence of characters that does not include ASCII space or control characters, 
                // and includes parentheses only if (a) they are backslash-escaped or (b) they are part of a 
                // balanced pair of unescaped parentheses that is not itself inside a balanced pair of unescaped 
                // parentheses. 
                bool hasEscape = false;
                int openedParent = 0;
                while (c != '\0')
                {
                    // Match opening and closing parenthesis
                    if (c == '(')
                    {
                        if (!hasEscape)
                        {
                            if (openedParent > 0)
                            {
                                break;
                            }
                            openedParent++;
                        }
                    }

                    if (c == ')')
                    {
                        if (!hasEscape)
                        {
                            openedParent--;
                            if (openedParent < 0)
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }

                    if (hasEscape && !CharHelper.IsAsciiPunctuation(c))
                    {
                        buffer.Append('\\');
                    }

                    // If we have an escape
                    if (c == '\\')
                    {
                        hasEscape = true;
                        c = text.NextChar();
                        continue;
                    }

                    hasEscape = false;

                    if (CharHelper.IsSpaceOrTab(c) || CharHelper.IsControl(c)) // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
                    {
                        isValid = true;
                        break;
                    }

                    buffer.Append(c);

                    c = text.NextChar();
                }
            }

            link = isValid ? buffer.ToString() : null;
            buffer.Clear();
            return isValid;
        }

        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] {'[', ']', '!'};
            }

            public override bool Match(MatchInlineState state)
            {
                var text = state.Lines;

                var c = text.Current;

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
                    if (text.Current == '(')
                    {
                        if (TryParseUrlAndTitle(text, out url, out title))
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