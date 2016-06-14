// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// Block parser for a <see cref="HtmlBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class HtmlBlockParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlockParser"/> class.
        /// </summary>
        public HtmlBlockParser()
        {
            OpeningCharacters = new[] { '<' };
        }
           
        public override BlockState TryOpen(BlockProcessor processor)
        {
            var result = MatchStart(processor);

            // An end-tag can occur on the same line, so we try to parse it here
            if (result == BlockState.Continue)
            {
                result = MatchEnd(processor, (HtmlBlock) processor.NewBlocks.Peek());
            }
            return result;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var htmlBlock = (HtmlBlock) block;
            return MatchEnd(processor, htmlBlock);
        }

        private BlockState MatchStart(BlockProcessor state)
        {
            if (state.IsCodeIndent)
            {
                return BlockState.None;
            }

            var line = state.Line;
            var startPosition = line.Start;
            line.NextChar();
            var result = TryParseTagType16(state, line, state.ColumnBeforeIndent, startPosition);

            // HTML blocks of type 7 cannot interrupt a paragraph:
            if (result == BlockState.None && !(state.CurrentBlock is ParagraphBlock))
            {
                result = TryParseTagType7(state, line, state.ColumnBeforeIndent, startPosition);
            }
            return result;
        }

        private BlockState TryParseTagType7(BlockProcessor state, StringSlice line, int startColumn, int startPosition)
        {
            var builder = StringBuilderCache.Local();
            var c = line.CurrentChar;
            var result = BlockState.None;
            if ((c == '/' && HtmlHelper.TryParseHtmlCloseTag(ref line, builder)) || HtmlHelper.TryParseHtmlTagOpenTag(ref line, builder))
            {
                // Must be followed by whitespace only
                bool hasOnlySpaces = true;
                c = line.CurrentChar;
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
                    c = line.NextChar();
                }

                if (hasOnlySpaces)
                {
                    result = CreateHtmlBlock(state, HtmlBlockType.NonInterruptingBlock, startColumn, startPosition);
                }
            }

            builder.Length = 0;
            return result;
        }

        private BlockState TryParseTagType16(BlockProcessor state, StringSlice line, int startColumn, int startPosition)
        {
            char c;
            c = line.CurrentChar;
            if (c == '!')
            {
                c = line.NextChar();
                if (c == '-' && line.PeekChar(1) == '-')
                {
                    return CreateHtmlBlock(state, HtmlBlockType.Comment, startColumn, startPosition); // group 2
                }
                if (c.IsAlphaUpper())
                {
                    return CreateHtmlBlock(state, HtmlBlockType.DocumentType, startColumn, startPosition); // group 4
                }
                if (c == '[' && line.Match("CDATA[", 1))
                {
                    return CreateHtmlBlock(state, HtmlBlockType.CData, startColumn, startPosition); // group 5
                }

                return BlockState.None;
            }

            if (c == '?')
            {
                return CreateHtmlBlock(state, HtmlBlockType.ProcessingInstruction, startColumn, startPosition); // group 3
            }

            var hasLeadingClose = c == '/';
            if (hasLeadingClose)
            {
                c = line.NextChar();
            }

            var tag = new char[10];
            var count = 0;
            for (; count < tag.Length; count++)
            {
                if (!c.IsAlphaNumeric())
                {
                    break;
                }
                tag[count] = Char.ToLowerInvariant(c);
                c = line.NextChar();
            }

            if (
                !(c == '>' || (!hasLeadingClose && c == '/' && line.PeekChar(1) == '>') || c.IsWhitespace() ||
                  c == '\0'))
            {
                return BlockState.None;
            }

            if (count == 0)
            {
                return BlockState.None;
            }

            var tagName = new string(tag, 0, count);
            var tagIndex = Array.BinarySearch(HtmlTags, tagName, StringComparer.Ordinal);
            if (tagIndex < 0)
            {
                return BlockState.None;
            }

            // Cannot start with </script </pre or </style
            if ((tagIndex == 45 || tagIndex == 46 || tagIndex == 49))
            {
                if (c == '/' || hasLeadingClose)
                {
                    return BlockState.None;
                }
                return CreateHtmlBlock(state, HtmlBlockType.ScriptPreOrStyle, startColumn, startPosition);
            }

            return CreateHtmlBlock(state, HtmlBlockType.InterruptingBlock, startColumn, startPosition);
        }

        private BlockState MatchEnd(BlockProcessor state, HtmlBlock htmlBlock)
        {
            state.GoToColumn(state.ColumnBeforeIndent);

            // Early exit if it is not starting by an HTML tag
            var line = state.Line;
            var c = line.CurrentChar;
            var result = BlockState.Continue;
            switch (htmlBlock.Type)
            {
                case HtmlBlockType.Comment:
                    if (line.Search("-->"))
                    {
                        htmlBlock.SourceEndPosition = line.End;
                        result = BlockState.Break;
                    }
                    break;
                case HtmlBlockType.CData:
                    if (line.Search("]]>"))
                    {
                        htmlBlock.SourceEndPosition = line.End;
                        result = BlockState.Break;
                    }
                    break;
                case HtmlBlockType.ProcessingInstruction:
                    if (line.Search("?>"))
                    {
                        htmlBlock.SourceEndPosition = line.End;
                        result = BlockState.Break;
                    }
                    break;
                case HtmlBlockType.DocumentType:
                    if (line.Search(">"))
                    {
                        htmlBlock.SourceEndPosition = line.End;
                        result = BlockState.Break;
                    }
                    break;
                case HtmlBlockType.ScriptPreOrStyle:
                    if (line.SearchLowercase("</script>") || line.SearchLowercase("</pre>") || line.SearchLowercase("</style>"))
                    {
                        htmlBlock.SourceEndPosition = line.End;
                        result = BlockState.Break;
                    }
                    break;
                case HtmlBlockType.InterruptingBlock:
                    if (state.IsBlankLine)
                    {
                        result = BlockState.BreakDiscard;
                    }
                    break;
                case HtmlBlockType.NonInterruptingBlock:
                    if (state.IsBlankLine)
                    {
                        result = BlockState.BreakDiscard;
                    }
                    break;
            }

            // Update only if we don't have a break discard
            if (result != BlockState.BreakDiscard)
            {
                htmlBlock.SourceEndPosition = line.End;
            }

            return result;
        }

        private BlockState CreateHtmlBlock(BlockProcessor state, HtmlBlockType type, int startColumn, int startPosition)
        {
            state.NewBlocks.Push(new HtmlBlock(this)
            {
                Column = startColumn,
                Type = type,
                SourceStartPosition = startPosition,
                // By default, setup to the end of line
                SourceEndPosition = startPosition + state.Line.End
            });
            return BlockState.Continue;
        }

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
    }
}