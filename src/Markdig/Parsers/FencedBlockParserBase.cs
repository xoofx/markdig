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
        /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
        public delegate bool InfoParserDelegate(BlockProcessor state, ref StringSlice line, IFencedBlock fenced);


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
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public abstract class FencedBlockParserBase<T> : FencedBlockParserBase where T : Block, IFencedBlock
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedBlockParserBase{T}"/> class.
        /// </summary>
        protected FencedBlockParserBase()
        {
            InfoParser = DefaultInfoParser;
            MinimumMatchCount = 3;
            MaximumMatchCount = Int32.MaxValue;
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
        /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
        public static bool DefaultInfoParser(BlockProcessor state, ref StringSlice line,
            IFencedBlock fenced)
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
                infoString = line.Text.Substring(line.Start, firstSpace - line.Start).Trim();

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

                argString = line.Text.Substring(firstSpace, line.End - firstSpace + 1).Trim();
            }
            else
            {
                infoString = line.ToString().Trim();
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

            var startPosition = processor.Start;

            // Match fenced char
            int count = 0;
            var line = processor.Line;
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
                fenced.Span.Start = startPosition;
                fenced.Span.End = line.Start;
            };

            // Try to parse any attached attributes
            if (TryParseAttributes != null)
            {
                TryParseAttributes(processor, ref line, fenced);
            }

            // If the info parser was not successfull, early exit
            if (InfoParser != null && !InfoParser(processor, ref line, fenced))
            {
                return BlockState.None;
            }

            // Add the language as an attribute by default
            if (!string.IsNullOrEmpty(fenced.Info))
            {
                if (string.IsNullOrEmpty(InfoPrefix))
                {
                    fenced.GetAttributes().AddClass(fenced.Info);
                }
                else
                {
                    fenced.GetAttributes().AddClass(InfoPrefix + fenced.Info);
                }
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
            var matchChar = fence.FencedChar;
            var c = processor.CurrentChar;

            // Match if we have a closing fence
            var line = processor.Line;
            while (c == matchChar)
            {
                c = line.NextChar();
                count--;
            }

            // If we have a closing fence, close it and discard the current line
            // The line must contain only fence opening character followed only by whitespaces.
            if (count <=0 && !processor.IsCodeIndent && (c == '\0' || c.IsWhitespace()) && line.TrimEnd())
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