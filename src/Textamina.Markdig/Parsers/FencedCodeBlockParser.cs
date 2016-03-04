// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Parser for a <see cref="FencedCodeBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.BlockParser" />
    public class FencedCodeBlockParser : BlockParser
    {
        /// <summary>
        /// Delegate used to parse the string on the first line after the fenced code block special characters (usually ` or ~)
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="line">The being processed line.</param>
        /// <param name="fenced">The fenced code block.</param>
        /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
        public delegate bool InfoParserDelegate(BlockParserState state, ref StringSlice line, FencedCodeBlock fenced);

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public FencedCodeBlockParser()
        {
            OpeningCharacters = new[] {'`', '~'};
            InfoParser = DefaultInfoParser;
        }

        /// <summary>
        /// Gets or sets the information parser.
        /// </summary>
        public InfoParserDelegate InfoParser { get; set; }

        /// <summary>
        /// The default parser for the information after the fenced code block special characters (usually ` or ~)
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="line">The line.</param>
        /// <param name="fenced">The fenced code block.</param>
        /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
        public static bool DefaultInfoParser(BlockParserState state, ref StringSlice line,
            FencedCodeBlock fenced)
        {
            string infoString;
            string argString = null;

            var c = line.CurrentChar;
            // An info string cannot contain any backsticks
            int firstSpace = -1;
            for (int i = line.Start; i <= line.End; i++)
            {
                c = line.Text[i];
                if (c == '`')
                {
                    return false;
                }

                if (firstSpace < 0 && c.IsSpaceOrTab())
                {
                    firstSpace = i;
                }
            }

            if (firstSpace > 0)
            {
                infoString = line.Text.Substring(line.Start, firstSpace - line.Start);

                // Skip any spaces after info string
                firstSpace++;
                while (true)
                {
                    c = line[firstSpace];
                    if (c.IsSpaceOrTab())
                    {
                        firstSpace++;
                    }
                    else
                    {
                        break;
                    }
                }

                argString = line.Text.Substring(firstSpace, line.End - firstSpace + 1);
            }
            else
            {
                infoString = line.ToString();
            }

            fenced.Language = HtmlHelper.Unescape(infoString);
            fenced.Arguments = HtmlHelper.Unescape(argString);

            return true;
        }

        public override BlockState TryOpen(BlockParserState state)
        {
            // We expect no indentation for a fenced code block.
            if (state.IsCodeIndent)
            {
                return BlockState.None;
            }

            // Match fenced char
            int count = 0;
            var line = state.Line;
            char c = line.CurrentChar;
            var matchChar = c;
            while (c != '\0')
            {
                if (c != matchChar)
                {
                    break;
                }
                count++;
                c = line.NextChar();
            }

            // A fenced codeblock requires at least 3 opening chars
            if (count < 3)
            {
                return BlockState.None;
            }

            // specs spaces: Is space and tabs? or only spaces? Use space and tab for this case
            line.TrimStart();

            var fenced = new FencedCodeBlock(this)
            {
                Column = state.Column,
                FencedChar = matchChar,
                FencedCharCount = count,
                IndentCount = state.Indent,
            };

            // If the info parser was not successfull, early exit
            if (InfoParser != null && !InfoParser(state, ref line, fenced))
            {
                return BlockState.None;
            }

            // Store the number of matched string into the context
            state.NewBlocks.Push(fenced);

            // Discard the current line as it is already parsed
            return BlockState.ContinueDiscard;
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            var fence = (FencedCodeBlock)block;
            var count = fence.FencedCharCount;
            var matchChar = fence.FencedChar;
            var c = state.CurrentChar;

            // Match if we have a closing fence
            var line = state.Line;
            while (c == matchChar)
            {
                c = line.NextChar();
                count--;
            }

            // If we have a closing fence, close it and discard the current line
            // The line must contain only fence opening character followed only by whitespaces.
            if (count <=0 && !state.IsCodeIndent && (c == '\0' || c.IsWhitespace()) && line.TrimEnd())
            {
                // Don't keep the last line
                return BlockState.BreakDiscard;
            }

            // Reset the indentation to the column before the indent
            state.GoToColumn(state.ColumnBeforeIndent);

            // Remove any indent spaces
            c = state.CurrentChar;
            var indentCount = fence.IndentCount;
            while (indentCount > 0 && c.IsSpace())
            {
                indentCount--;
                c = state.NextChar();
            }

            return BlockState.Continue;
        }
    }
}