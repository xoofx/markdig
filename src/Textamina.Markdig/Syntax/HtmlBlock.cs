using System;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class HtmlBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        public HtmlBlock()
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
           
            public override MatchLineResult Match(ref MatchLineState state)
            {
                var htmlBlock = state.Block as HtmlBlock;
                if (htmlBlock == null)
                {
                    var result = MatchStart(ref state);
                    // An end-tag can occur on the same line
                    if (result == MatchLineResult.Continue)
                    {
                        return MatchEnd(ref state, (HtmlBlock) state.Block);
                    }
                    return result;
                }

                return MatchEnd(ref state, htmlBlock);
            }

            private MatchLineResult MatchStart(ref MatchLineState state)
            {
                var liner = state.Line;
                int index = 0;

                for (int i = 0; i < 3; i++)
                {
                    if (!Utility.IsSpace(liner.PeekChar(index)))
                    {
                        break;
                    }
                    index++;
                }

                // Early exit if it is not starting by an HTML tag
                var c = liner.PeekChar(index++);
                if (c != '<')
                {
                    return MatchLineResult.None;
                }

                c = liner.PeekChar(index);
                if (c == '!')
                {
                    c = liner.PeekChar(index + 1);
                    if (c == '-' && liner.PeekChar(index + 2) == '-')
                    {
                        return CreateHtmlBlock(ref state, HtmlBlockType.Comment); // group 2
                    }
                    if (Utility.IsAlphaUpper(c))
                    {
                        return CreateHtmlBlock(ref state, HtmlBlockType.DocumentType); // group 4
                    }
                    if (c == '[' && liner.Match("CDATA[", 3))
                    {
                        return CreateHtmlBlock(ref state, HtmlBlockType.CData); // group 5
                    }

                    return MatchLineResult.None;
                }

                if (c == '?')
                {
                    return CreateHtmlBlock(ref state, HtmlBlockType.ProcessingInstruction); // group 3
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
                    if (!Utility.IsAlphaNumeric(c))
                    {
                        break;
                    }
                    tag[count] = char.ToLowerInvariant(c);
                }

                if (!(c == '>' || (!hasLeadingClose && c == '/' && liner.PeekChar(index + 1) == '>') || Utility.IsWhiteSpace(c) || c == '\0'))
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
                    return CreateHtmlBlock(ref state, HtmlBlockType.ScriptPreOrStyle);
                }

                return CreateHtmlBlock(ref state, HtmlBlockType.InterruptingBlock);
            }

            private MatchLineResult MatchEnd(ref MatchLineState state, HtmlBlock htmlBlock)
            {
                var liner = state.Line;

                // Early exit if it is not starting by an HTML tag
                var c = liner.Current;
                switch (htmlBlock.Type)
                {
                    case HtmlBlockType.Comment:
                        if (liner.Search("-->"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.CData:
                        if (liner.Search("]]>"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.ProcessingInstruction:
                        if (liner.Search("?>"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.DocumentType:
                        if (liner.Search(">"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.ScriptPreOrStyle:
                        // TODO: could be optimized with a dedicated parser
                        if (liner.SearchLowercase("</script>") || liner.SearchLowercase("</pre>") || liner.SearchLowercase("</style>"))
                        {
                            return MatchLineResult.Last;
                        }
                        break;
                    case HtmlBlockType.InterruptingBlock:
                        if (liner.IsBlankLine())
                        {
                            return MatchLineResult.LastDiscard;
                        }
                        break;
                    case HtmlBlockType.NonInterruptingBlock:
                        // TODO
                        break;
                }

                return MatchLineResult.Continue;
            }

            private MatchLineResult CreateHtmlBlock(ref MatchLineState state, HtmlBlockType type)
            {
                state.Block = new HtmlBlock() {Type = type};
                return MatchLineResult.Continue;
            }
        }
    }
}