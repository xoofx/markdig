using System;
using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class LinkInline : ContainerInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        [ThreadStatic]
        private static readonly StringBuilder TempBuilder = new StringBuilder();

        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] {'[', ']', '!'};
            }

            public override bool Match(MatchInlineState state)
            {
                var lines = state.Lines;

                var c = lines.Current;

                bool isImage = false;
                if (c == '!')
                {
                    isImage = true;
                    c = lines.NextChar();
                    if (c != '[')
                    {
                        return false;
                    }
                }

                if (c == '[')
                {
                    state.Inline = new LinkDelimiterInline(this)
                    {
                        Type = DelimiterType.Open,
                        IsImage = isImage
                    };
                    return true;
                }
                else if (c == ']')
                {
                    if (state.Inline != null)
                    {
                        LinkDelimiterInline firstParent = null;
                        foreach (var parent in state.Inline.FindParentOfType<LinkDelimiterInline>())
                        {
                            firstParent = parent;
                            break;
                        }

                        // This will be matched as a literal
                        if (firstParent != null)
                        {
                            // TODO: continue parsing of ]

                            // We have a nested [ ]
                            // firstParent.Remove();
                            // The opening [ will be transformed to a literal followed by all the childrens of the [ 

                            var literal = new LiteralInline() {Content = firstParent.IsImage ? "![" : "["};
                            state.Inline = firstParent.ReplaceBy(literal);

                            return true;
                        }
                        //else
                        //{
                        //    var link = firstParent.IsImage
                        //        ? (LinkInline) new ImageLinkInline()
                        //        : new TextLinkInline();

                        //    // 1. Process all delimiters inside firstParent to convert them to inlines
                        //    // 2. Replace firstParent with link, and move all child to this one

                        //    // TODO: continue parsing of ]

                        //    if (state.Inline == firstParent)
                        //    {
                        //        state.Inline = link;
                        //    }
                        //}
                    }
                    return false;

                    // Match a close tag
                }


                // We don't have an emphasis
                return false;
            }

            private bool TryParseLinkUrl(MatchInlineState state)
            {


                return false;
            }


            private bool TryParseLinkTitle(MatchInlineState state)
            {


                return false;
            }

        }


        public static bool TryParseLinkTitle(StringLineGroup text, out string title)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            // a sequence of zero or more characters between straight double-quote characters ("), including a " character only if it is backslash-escaped, or
            // a sequence of zero or more characters between straight single-quote characters ('), including a ' character only if it is backslash-escaped, or
            // a sequence of zero or more characters between matching parentheses ((...)), including a ) character only if it is backslash-escaped.
            title = null;

            var c = text.Current;
            if (c == '\'' || c == '"')
            {
                
            }

            return false;
        }


        public static bool TryParseLinkDestination(StringLineGroup text, out string link)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var destination = TempBuilder;
            destination.Clear();
            link = null;

            var c = text.Current;

            // a sequence of zero or more characters between an opening < and a closing > 
            // that contains no spaces, line breaks, or unescaped < or > characters, or
            if (c == '<')
            {
                bool nextEscape = false;
                do
                {
                    c = text.NextChar();
                    if (!nextEscape && c == '>')
                    {
                        link = destination.ToString();
                        destination.Clear();
                        return true;
                    }

                    if (!nextEscape && c == '<')
                    {
                        break;
                    }

                    if (c == '\\')
                    {
                        nextEscape = true;
                        continue;
                    }

                    nextEscape = false;

                    if (Utility.IsWhiteSpace(c)) // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
                    {
                        break;
                    }

                    destination.Append(c);

                } while (c != '\0');
            }
            else
            {
                // a nonempty sequence of characters that does not include ASCII space or control characters, 
                // and includes parentheses only if (a) they are backslash-escaped or (b) they are part of a 
                // balanced pair of unescaped parentheses that is not itself inside a balanced pair of unescaped 
                // parentheses. 
                bool isEscaped = false;
                int openedParent = 0;
                do
                {
                    c = text.NextChar();

                    // Match opening and closing parenthesis
                    if (c == '(')
                    {
                        if (!isEscaped)
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
                        if (!isEscaped)
                        {
                            openedParent--;
                            if (openedParent < 0)
                            {
                                break;
                            }
                        }
                    }

                    // If we have an escape
                    if (c == '\\')
                    {
                        isEscaped = true;
                        continue;
                    }

                    isEscaped = false;

                    var isSpace = Utility.IsSpaceOrTab(c);
                    if (isSpace || Utility.IsControl(c)) // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
                    {
                        if (isSpace && destination.Length > 0)
                        {
                            link = destination.ToString();
                            destination.Clear();
                            return true;
                        }
                        break;
                    }

                    destination.Append(c);

                } while (c != '\0');
            }

            // Clear the StringBuilder even after in order to avoid storing things around after using it
            destination.Clear();
            return false;
        }
    }
}