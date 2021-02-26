// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    public abstract class FencedBlockParserBase : BlockParser, IAttributesParseable
    {
        /// <summary>
        /// Delegate used to parse the string on the first line after the fenced code block special characters (usually ` or ~)
        /// </summary>
        /// <param name="state">The parser processor.</param>
        /// <param name="line">The being processed line.</param>
        /// <param name="fenced">The fenced code block.</param>
        /// <param name="openingCharacter">The opening character for the fenced code block (usually ` or ~)</param>
        /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
        public delegate bool InfoParserDelegate(BlockProcessor state, ref StringSlice line, IFencedBlock fenced, char openingCharacter);


        /// <summary>
        /// Gets or sets the information parser.
        /// </summary>
        public InfoParserDelegate InfoParser { get; set; }

        /// <summary>
        /// A delegates that allows to process attached attributes
        /// </summary>
        public TryParseAttributesDelegate TryParseAttributes { get; set; }
    }

    /// <summary>
    /// Base parser for fenced blocks (opened by 3 or more character delimiters on a first line, and closed by at least the same number of delimiters)
    /// </summary>
    /// <seealso cref="BlockParser" />
    public abstract class FencedBlockParserBase<T> : FencedBlockParserBase where T : Block, IFencedBlock
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedBlockParserBase{T}"/> class.
        /// </summary>
        protected FencedBlockParserBase()
        {
            InfoParser = DefaultInfoParser;
            MinimumMatchCount = 3;
            MaximumMatchCount = int.MaxValue;
        }

        /// <summary>
        /// Gets or sets the language prefix (default is "language-")
        /// </summary>
        public string InfoPrefix { get; set; }

        public int MinimumMatchCount { get; set; }

        public int MaximumMatchCount { get; set; }

        /// <summary>
        /// The default parser for the information after the fenced code block special characters (usually ` or ~)
        /// </summary>
        /// <param name="state">The parser processor.</param>
        /// <param name="line">The line.</param>
        /// <param name="fenced">The fenced code block.</param>
        /// <param name="openingCharacter">The opening character for this fenced code block.</param>
        /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
        public static bool DefaultInfoParser(BlockProcessor state, ref StringSlice line, IFencedBlock fenced, char openingCharacter)
        {
            string infoString;
            string argString = null;

            // An info string cannot contain any backticks (unless it is a tilde block)
            int firstSpace = -1;
            if (openingCharacter == '`')
            {
                for (int i = line.Start; i <= line.End; i++)
                {
                    char c = line.Text[i];
                    if (c == '`')
                    {
                        return false;
                    }

                    if (firstSpace < 0 && c.IsSpaceOrTab())
                    {
                        firstSpace = i;
                    }
                }
            }
            else
            {
                for (int i = line.Start; i <= line.End; i++)
                {
                    if (line.Text[i].IsSpaceOrTab())
                    {
                        firstSpace = i;
                        break;
                    }
                }
            }

            if (firstSpace > 0)
            {
                infoString = line.Text.Substring(line.Start, firstSpace - line.Start).Trim();

                // Skip any spaces after info string
                firstSpace++;
                while (firstSpace <= line.End)
                {
                    char c = line[firstSpace];
                    if (c.IsSpaceOrTab())
                    {
                        firstSpace++;
                    }
                    else
                    {
                        break;
                    }
                }

                argString = line.Text.Substring(firstSpace, line.End - firstSpace + 1).Trim();
            }
            else
            {
                var lineCopy = line;
                lineCopy.Trim();
                infoString = lineCopy.ToString();
            }

            fenced.Info = HtmlHelper.Unescape(infoString);
            fenced.Arguments = HtmlHelper.Unescape(argString);

            return true;
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // We expect no indentation for a fenced code block.
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            // Match fenced char
            var line = processor.Line;
            char matchChar = line.CurrentChar;
            int count = line.CountAndSkipChar(matchChar);

            // A fenced codeblock requires at least 3 opening chars
            if (count < MinimumMatchCount || count > MaximumMatchCount)
            {
                return BlockState.None;
            }

            // specs spaces: Is space and tabs? or only spaces? Use space and tab for this case
            line.TrimStart();

            var fenced = CreateFencedBlock(processor);
            {
                fenced.Column = processor.Column;
                fenced.FencedChar = matchChar;
                fenced.FencedCharCount = count;
                fenced.Span.Start = processor.Start;
                fenced.Span.End = line.Start;
            };

            // Try to parse any attached attributes
            TryParseAttributes?.Invoke(processor, ref line, fenced);

            // If the info parser was not successfull, early exit
            if (InfoParser != null && !InfoParser(processor, ref line, fenced, matchChar))
            {
                return BlockState.None;
            }

            // Add the language as an attribute by default
            if (!string.IsNullOrEmpty(fenced.Info))
            {
                fenced.GetAttributes().AddClass(InfoPrefix + fenced.Info);
            }

            // Store the number of matched string into the context
            processor.NewBlocks.Push(fenced);

            // Discard the current line as it is already parsed
            return BlockState.ContinueDiscard;
        }

        protected abstract T CreateFencedBlock(BlockProcessor processor);

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var fence = (IFencedBlock)block;
            var count = fence.FencedCharCount;

            // Match if we have a closing fence
            var line = processor.Line;
            count -= line.CountAndSkipChar(fence.FencedChar);

            char c = line.CurrentChar;

            // If we have a closing fence, close it and discard the current line
            // The line must contain only fence opening character followed only by whitespaces.
            if (count <= 0 && !processor.IsCodeIndent && (c == '\0' || c.IsWhitespace()) && line.TrimEnd())
            {
                block.UpdateSpanEnd(line.Start - 1);

                // Don't keep the last line
                return BlockState.BreakDiscard;
            }

            // Reset the indentation to the column before the indent
            processor.GoToColumn(processor.ColumnBeforeIndent);

            return BlockState.Continue;
        }
    }
}