using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class ThematicBreakParser : BlockParser
    {
        public static readonly ThematicBreakParser Default = new ThematicBreakParser();

        public ThematicBreakParser()
        {
            OpeningCharacters = new[] {'-', '_', '*'};
        }

        public override BlockState TryOpen(BlockParserState state)
        {
            if (state.IsCodeIndent)
            {
                return BlockState.None;
            }

            // 4.1 Thematic breaks 
            // A line consisting of 0-3 spaces of indentation, followed by a sequence of three or more matching -, _, or * characters, each followed optionally by any number of spaces
            int breakCharCount = 1;
            var breakChar = state.CurrentChar;
            bool hasSpacesSinceLastMatch = false;
            bool hasInnerSpaces = false;
            int offset = 0;
            var c = state.PeekChar(offset++);
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
                else if (c.IsSpace())
                {
                    hasSpacesSinceLastMatch = true;
                }
                else
                {
                    return BlockState.None;
                }

                c = state.PeekChar(offset);
                offset++;
            }

            // If it as less than 3 chars or it is a setex heading and we are already in a paragraph, let the paragraph handle it
            var previousParagraph = state.LastBlock as ParagraphBlock;

            var isSetexHeading = previousParagraph != null && breakChar == '-' && !hasInnerSpaces;
            if (isSetexHeading)
            {
                var parent = previousParagraph.Parent;
                if (parent is QuoteBlock || (parent is ListItemBlock && previousParagraph.Column != state.Column))
                {
                    isSetexHeading = false;
                }
            }

            if (breakCharCount < 3 || isSetexHeading)
            {
                return BlockState.None;
            }

            // Push a new block
            state.NewBlocks.Push(new ThematicBreakBlock(this) { Column = state.Column });
            return BlockState.BreakDiscard;
        }
    }
}