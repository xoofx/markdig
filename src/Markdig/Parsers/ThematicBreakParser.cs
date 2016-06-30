// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// A block parser for a <see cref="ThematicBreakBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class ThematicBreakParser : BlockParser
    {
        /// <summary>
        /// A singleton instance used by other parsers.
        /// </summary>
        public static readonly ThematicBreakParser Default = new ThematicBreakParser();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThematicBreakParser"/> class.
        /// </summary>
        public ThematicBreakParser()
        {
            OpeningCharacters = new[] {'-', '_', '*'};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            var startPosition = processor.Start;

            var line = processor.Line;

            // 4.1 Thematic breaks 
            // A line consisting of 0-3 spaces of indentation, followed by a sequence of three or more matching -, _, or * characters, each followed optionally by any number of spaces
            int breakCharCount = 0;
            var breakChar = line.CurrentChar;
            bool hasSpacesSinceLastMatch = false;
            bool hasInnerSpaces = false;
            var c = breakChar;
            while (c != '\0')
            {
                if (c == breakChar)
                {
                    if (hasSpacesSinceLastMatch)
                    {
                        hasInnerSpaces = true;
                    }

                    breakCharCount++;
                }
                else if (c.IsSpaceOrTab())
                {
                    hasSpacesSinceLastMatch = true;
                }
                else
                {
                    return BlockState.None;
                }

                c = line.NextChar();
            }

            // If it as less than 3 chars or it is a setex heading and we are already in a paragraph, let the paragraph handle it
            var previousParagraph = processor.CurrentBlock as ParagraphBlock;

            var isSetexHeading = previousParagraph != null && breakChar == '-' && !hasInnerSpaces;
            if (isSetexHeading)
            {
                var parent = previousParagraph.Parent;
                if (parent is QuoteBlock || (parent is ListItemBlock && previousParagraph.Column != processor.Column))
                {
                    isSetexHeading = false;
                }
            }

            if (breakCharCount < 3 || isSetexHeading)
            {
                return BlockState.None;
            }

            // Push a new block
            processor.NewBlocks.Push(new ThematicBreakBlock(this)
            {
                Column = processor.Column,
                Span = new SourceSpan(startPosition, line.End)
            });
            return BlockState.BreakDiscard;
        }
    }
}