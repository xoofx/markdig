using System;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class HtmlBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        public HtmlBlock(BlockParser parser) : base(parser)
        {
            // We don't process inline of an html block, as we will copy the content as-is
            NoInline = true; 
        }

        public HtmlBlockType Type { get; set; }

        private class ParserInternal : BlockParser
        {
            private static readonly string[] HtmlTags =
            {
                "address",     // 0
                "article",     // 1
                "aside",       // 2
                "base",        // 3
                "basefont",    // 4
                "blockquote",  // 5
                "body",        // 6
                "caption",     // 7
                "center",      // 8
                "col",         // 9
                "colgroup",    // 10
                "dd",          // 11
                "details",     // 12
                "dialog",      // 13
                "dir",         // 14
                "div",         // 15
                "dl",          // 16
                "dt",          // 17
                "fieldset",    // 18
                "figcaption",  // 19
                "figure",      // 20
                "footer",      // 21
                "form",        // 22
                "frame",       // 23
                "frameset",    // 24
                "h1",          // 25
                "head",        // 26
                "header",      // 27
                "hr",          // 28
                "html",        // 29
                "iframe",      // 30
                "legend",      // 31
                "li",          // 32
                "link",        // 33
                "main",        // 34
                "menu",        // 35
                "menuitem",    // 36
                "meta",        // 37
                "nav",         // 38
                "noframes",    // 39
                "ol",          // 40
                "optgroup",    // 41
                "option",      // 42
                "p",           // 43
                "param",       // 44
                "pre",         // 45   <- special group 1
                "script",      // 46   <- special group 1
                "section",     // 47
                "source",      // 48
                "style",       // 49   <- special group 1
                "summary",     // 50
                "table",       // 51
                "tbody",       // 52
                "td",          // 53
                "tfoot",       // 54
                "th",          // 55
                "thead",       // 56
                "title",       // 57
                "tr",          // 58
                "track",       // 59
                "ul",          // 60
            };
           
            public override MatchLineResult Match(BlockParserState state)
            {
                var htmlBlock = state.Pending as HtmlBlock;
                if (htmlBlock == null)
                {
                    var result = MatchStart(state);
                    // An end-tag can occur on the same line
                    if (result == MatchLineResult.Continue)
                    {
                        return MatchEnd(state, (HtmlBlock) state.NewBlocks.Peek());
                    }
                    return result;
                }

                return MatchEnd(state, htmlBlock);
            }

            private MatchLineResult MatchStart(BlockParserState state)
            {
                int index = 0;

                for (int i = 0; i < 3; i++)
                {
                    if (!state.Line.PeekChar(index).IsSpace())
                    {
                        break;
                    }
                    index++;
                }

                // Early exit if it is not starting by an HTML tag
                var column = index;
                var c = state.Line.PeekChar(index++);
                if (c != '<')
                {
                    return MatchLineResult.None;
                }

                var result = TryParseTagType16(state, ref state.Line, index, column);

                // HTML blocks of type 7 cannot interrupt a paragraph:
                if (result == MatchLineResult.None && !(state.LastBlock is ParagraphBlock))
                {
                    result = TryParseTagType7(state, ref state.Line, index, column);
                }

                return result;
            }

            private MatchLineResult TryParseTagType7(BlockParserState state, ref StringSlice liner, int index, int startColumn)
            {
                var builder = StringBuilderCache.Local();
                liner.Start = index;
                var c = liner.CurrentChar;
                var result = MatchLineResult.None;
                if ((c == '/' && HtmlHelper.TryParseHtmlCloseTag(liner, builder)) || HtmlHelper.TryParseHtmlTagOpenTag(liner, builder))
                {
                    // Must be followed by whitespace only
                    bool hasOnlySpaces = true;
                    c = liner.CurrentChar;
                    while (true)
                    {
                        if (c == '\0')
                        {
                            break;
                        }
                        if (!c.IsWhitespace())
                        {
                            hasOnlySpaces = false;
                            break;
                        }
                        c = liner.NextChar();
                    }

                    if (hasOnlySpaces)
                    {
                        result = CreateHtmlBlock(state, HtmlBlockType.NonInterruptingBlock, startColumn);
                    }
                }

                builder.Clear();
                return result;
            }

            private MatchLineResult TryParseTagType16(BlockParserState state, ref StringSlice liner, int index, int startColumn)
            {
                char c;
                c = liner.PeekChar(index);
                if (c == '!')
                {
                    c = liner.PeekChar(index + 1);
                    if (c == '-' && liner.PeekChar(index + 2) == '-')
                    {
                        return CreateHtmlBlock(state, HtmlBlockType.Comment, startColumn); // group 2
                    }
                    if (c.IsAlphaUpper())
                    {
                        return CreateHtmlBlock(state, HtmlBlockType.DocumentType, startColumn); // group 4
                    }
                    if (c == '[' && liner.Match("CDATA[", 3))
                    {
                        return CreateHtmlBlock(state, HtmlBlockType.CData, startColumn); // group 5
                    }

                    return MatchLineResult.None;
                }

                if (c == '?')
                {
                    return CreateHtmlBlock(state, HtmlBlockType.ProcessingInstruction, startColumn); // group 3
                }

                var hasLeadingClose = c == '/';
                if (hasLeadingClose)
                {
                    index++;
                }

                var tag = new char[10];
                var count = 0;
                for (; count < tag.Length; index++, count++)
                {
                    c = liner.PeekChar(index);
                    if (!c.IsAlphaNumeric())
                    {
                        break;
                    }
                    tag[count] = char.ToLowerInvariant(c);
                }

                if (
                    !(c == '>' || (!hasLeadingClose && c == '/' && liner.PeekChar(index + 1) == '>') || c.IsWhitespace() ||
                      c == '\0'))
                {
                    return MatchLineResult.None;
                }

                if (count == 0)
                {
                    return MatchLineResult.None;
                }

                var tagName = new string(tag, 0, count);
                var tagIndex = Array.BinarySearch(HtmlTags, tagName, StringComparer.Ordinal);
                if (tagIndex < 0)
                {
                    return MatchLineResult.None;
                }

                // Cannot start with </script </pre or </style
                if ((tagIndex == 45 || tagIndex == 46 || tagIndex == 49))
                {
                    if (c == '/' || hasLeadingClose)
                    {
                        return MatchLineResult.None;
                    }
                    return CreateHtmlBlock(state, HtmlBlockType.ScriptPreOrStyle, startColumn);
                }

                return CreateHtmlBlock(state, HtmlBlockType.InterruptingBlock, startColumn);
            }

            private MatchLineResult MatchEnd(BlockParserState state, HtmlBlock htmlBlock)
            {
                // Early exit if it is not starting by an HTML tag
                var c = state.Line.CurrentChar;
                switch (htmlBlock.Type)
                {
                    case HtmlBlockType.Comment:
                        if (state.Line.Search("-->"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.CData:
                        if (state.Line.Search("]]>"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.ProcessingInstruction:
                        if (state.Line.Search("?>"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.DocumentType:
                        if (state.Line.Search(">"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.ScriptPreOrStyle:
                        // TODO: could be optimized with a dedicated parser
                        if (state.Line.SearchLowercase("</script>") || state.Line.SearchLowercase("</pre>") || state.Line.SearchLowercase("</style>"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.InterruptingBlock:
                        if (state.IsBlankLine)
                        {
                            return MatchLineResult.LastDiscard;
                        }
                        break;
                    case HtmlBlockType.NonInterruptingBlock:
                        if (state.IsBlankLine)
                        {
                            return MatchLineResult.LastDiscard;
                        }
                        break;
                }

                return MatchLineResult.Continue;
            }

            private MatchLineResult CreateHtmlBlock(BlockParserState state, HtmlBlockType type, int startColumn)
            {
                state.NewBlocks.Push(new HtmlBlock(this) {Column = startColumn, Type = type});
                return MatchLineResult.Continue;
            }
        }
    }
}