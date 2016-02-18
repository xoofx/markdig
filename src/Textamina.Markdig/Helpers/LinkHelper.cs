


using System;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Helpers
{
    public static class LinkHelper
    {
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
            var c = text.CurrentChar;
            link = null;
            title = null;

            // 1. An inline link consists of a link text followed immediately by a left parenthesis (, 
            if (c == '(')
            {
                text.NextChar();
                text.SkipWhiteSpaces();

                if (TryParseUrl(text, out link))
                {
                    var hasWhiteSpaces = text.SkipWhiteSpaces();

                    c = text.CurrentChar;
                    if (c == ')')
                    {
                        isValid = true;
                    }
                    else if (hasWhiteSpaces)
                    {
                        c = text.CurrentChar;
                        if (c == ')')
                        {
                            isValid = true;
                        }
                        else if (TryParseTitle(text, out title))
                        {
                            text.SkipWhiteSpaces();
                            c = text.CurrentChar;

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
                title = title ?? String.Empty;
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
            var c = text.CurrentChar;
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

                    if (hasEscape && !c.IsAsciiPunctuation())
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

        public static bool TryParseUrl(StringLineGroup text, out string link)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            bool isValid = false;
            var buffer = StringBuilderCache.Local();
            buffer.Clear();

            var c = text.CurrentChar;

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

                    if (hasEscape && !c.IsAsciiPunctuation())
                    {
                        buffer.Append('\\');
                    }

                    if (c == '\\')
                    {
                        hasEscape = true;
                        continue;
                    }

                    hasEscape = false;

                    if (c.IsWhitespace()) // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
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

                    if (hasEscape && !c.IsAsciiPunctuation())
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

                    if (c.IsSpaceOrTab() || c.IsControl()) // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
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

        public static bool TryParseLabelUrlAndTitle(StringLineGroup lines, out string label, out string url,
            out string title)
        {
            url = null;
            title = null;
            if (!TryParseLabel(lines, out label))
            {
                return false;
            }

            if (lines.CurrentChar != ':')
            {
                label = null;
                return false;
            }
            lines.NextChar(); // Skip ':'

            if (!TryParseUrlAndTitle(lines, out url, out title))
            {
                label = null;
                return false;
            }
            return true;
        }

        public static bool TryParseLabel(StringLineGroup lines, out string label)
        {
            label = null;
            char c = lines.CurrentChar;
            if (c != '[')
            {
                return false;
            }
            var buffer = StringBuilderCache.Local();

            bool hasEscape = false;
            bool previousWhitespace = true;
            bool hasNonWhiteSpace = false;
            bool isValid = false;
            while (true)
            {
                c = lines.NextChar();
                if (c == '\0')
                {
                    break;
                }

                if (hasEscape)
                {
                    if (c != '[' && c != ']')
                    {
                        break;
                    }
                }
                else
                {
                    if (c == '[')
                    {
                        break;
                    }

                    if (c == ']')
                    {
                        lines.NextChar(); // Skip ]
                        if (hasNonWhiteSpace)
                        {
                            // Remove trailing spaces
                            for (int i = buffer.Length - 1; i >= 0; i--)
                            {
                                if (!buffer[i].IsWhitespace())
                                {
                                    break;
                                }
                                buffer.Length = i;
                            }

                            // Only valid if buffer is less than 1000 characters
                            if (buffer.Length <= 999)
                            {
                                label = buffer.ToString();
                                isValid = true;
                            }
                        }
                        break;
                    }
                }

                var isWhitespace = c.IsWhitespace();
                if (isWhitespace)
                {
                    // Replace any whitespace by a single ' '
                    c = ' ';
                }

                if (c == '\\')
                {
                    hasEscape = true;
                }
                else
                {
                    hasEscape = false;

                    if (!previousWhitespace || !isWhitespace)
                    {
                        buffer.Append(c);
                        if (!isWhitespace)
                        {
                            hasNonWhiteSpace = true;
                        }
                    }
                }
                previousWhitespace = isWhitespace;
            }

            buffer.Clear();

            return isValid;
        }
    }
}