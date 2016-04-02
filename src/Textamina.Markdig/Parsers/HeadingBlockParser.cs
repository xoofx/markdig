// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Block parser for a <see cref="HeadingBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.BlockParser" />
    public class HeadingBlockParser : BlockParser, IAttributesParseable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadingBlockParser"/> class.
        /// </summary>
        public HeadingBlockParser()
        {
            OpeningCharacters = new[] {'#'};
        }

        /// <summary>
        /// A delegates that allows to process attached attributes after #
        /// </summary>
        public TryParseAttributesDelegate TryParseAttributes { get; set; }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // If we are in a CodeIndent, early exit
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            // 4.2 ATX headings
            // An ATX heading consists of a string of characters, parsed as inline content, 
            // between an opening sequence of 1–6 unescaped # characters and an optional 
            // closing sequence of any number of unescaped # characters. The opening sequence 
            // of # characters must be followed by a space or by the end of line. The optional
            // closing sequence of #s must be preceded by a space and may be followed by spaces
            // only. The opening # character may be indented 0-3 spaces. The raw contents of 
            // the heading are stripped of leading and trailing spaces before being parsed as 
            // inline content. The heading level is equal to the number of # characters in the 
            // opening sequence.
            var column = processor.Column;
            var line = processor.Line;
            var c = line.CurrentChar;
            var matchingChar = c;

            int leadingCount = 0;
            while (c != '\0' && leadingCount <= 6)
            {
                if (c != matchingChar)
                {
                    break;
                }
                c = line.NextChar();
                leadingCount++;
            }

            // A space is required after leading #
            if (leadingCount > 0 && leadingCount <= 6 && (c.IsSpace() || c == '\0'))
            {
                // Move to the content
                processor.Line.Start = line.Start + 1;
                var headingBlock = new HeadingBlock(this)
                {
                    HeaderChar = matchingChar,
                    Level = leadingCount,
                    Column = column
                };
                processor.NewBlocks.Push(headingBlock);

                // Gives a chance to parse attributes
                if (TryParseAttributes != null)
                {
                    TryParseAttributes(processor, ref processor.Line, headingBlock);
                }

                // The optional closing sequence of #s must be preceded by a space and may be followed by spaces only.
                int endState = 0;
                int countClosingTags = 0;
                for (int i = processor.Line.End; i >= processor.Line.Start - 1; i--)  // Go up to Start - 1 in order to match the space after the first ###
                {
                    c = processor.Line.Text[i];
                    if (endState == 0)
                    {
                        if (c.IsSpace()) // TODO: Not clear if it is a space or space+tab in the specs
                        {
                            continue;
                        }
                        endState = 1;
                    }
                    if (endState == 1)
                    {
                        if (c == matchingChar)
                        {
                            countClosingTags++;
                            continue;
                        }

                        if (countClosingTags > 0)
                        {
                            if (c.IsSpace())
                            {
                                processor.Line.End = i - 1;
                            }
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // We expect a single line, so don't continue
                return BlockState.Break;
            }

            // Else we don't have an header
            return BlockState.None;
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            var heading = (HeadingBlock)block;
            heading.Lines.Trim();
            return true;
        }
    }
}