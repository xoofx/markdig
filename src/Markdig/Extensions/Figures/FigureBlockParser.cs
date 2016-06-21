// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Figures
{
    /// <summary>
    /// The block parser for a <see cref="Figure"/> block.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class FigureBlockParser : BlockParser
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedBlockParserBase"/> class.
        /// </summary>
        public FigureBlockParser()
        {
            OpeningCharacters = new [] {'^'};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // We expect no indentation for a figure block.
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            // Match fenced char
            int count = 0;
            var column = processor.Column;
            var line = processor.Line;
            var startPosition = line.Start;
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

            // Requires at least 3 opening chars
            if (count < 3)
            {
                return BlockState.None;
            }

            var figure = new Figure(this)
            {
                Span = new SourceSpan(startPosition, line.End),
                Line = processor.LineIndex,
                Column = processor.Column,
                OpeningCharacter = matchChar,
                OpeningCharacterCount = count
            };

            line.TrimStart();
            if (!line.IsEmpty)
            {
                var caption = new FigureCaption(this)
                {
                    Span = new SourceSpan(line.Start, line.End),
                    Line = processor.LineIndex,
                    Column = column + line.Start - startPosition,
                    IsOpen = false
                };
                caption.AppendLine(ref line, caption.Column, processor.LineIndex, processor.CurrentLineStartPosition);
                figure.Add(caption);
            }
            processor.NewBlocks.Push(figure);

            // Discard the current line as it is already parsed
            return BlockState.ContinueDiscard;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var figure = (Figure)block;
            var count = figure.OpeningCharacterCount;
            var matchChar = figure.OpeningCharacter;
            var c = processor.CurrentChar;

            var column = processor.Column;
            // Match if we have a closing fence
            var line = processor.Line;
            var startPosition = line.Start;
            while (c == matchChar)
            {
                c = line.NextChar();
                count--;
            }

            // If we have a closing fence, close it and discard the current line
            // The line must contain only fence opening character followed only by whitespaces.
            if (count <= 0 && !processor.IsCodeIndent)
            {
                line.TrimStart();
                if (!line.IsEmpty)
                {
                    var caption = new FigureCaption(this)
                    {
                        Span = new SourceSpan(line.Start, line.End),
                        Line = processor.LineIndex,
                        Column = column + line.Start - startPosition,
                        IsOpen = false
                    };
                    caption.AppendLine(ref line, caption.Column, processor.LineIndex, processor.CurrentLineStartPosition);
                    figure.Add(caption);
                }

                figure.UpdateSpanEnd(line.End);

                // Don't keep the last line
                return BlockState.BreakDiscard;
            }

            // Reset the indentation to the column before the indent
            processor.GoToColumn(processor.ColumnBeforeIndent);

            figure.UpdateSpanEnd(line.End);

            return BlockState.Continue;
        }
    }
}