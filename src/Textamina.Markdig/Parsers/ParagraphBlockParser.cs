using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class ParagraphBlockParser : BlockParser
    {
        public override BlockState TryOpen(BlockParserState state)
        {
            if (state.IsBlankLine)
            {
                return BlockState.None;
            }

            // We continue trying to match by default
            state.NewBlocks.Push(new ParagraphBlock(this) {Column = state.Column});
            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            if (state.IsBlankLine)
            {
                return BlockState.BreakDiscard;
            }

            if (!state.IsCodeIndent && !(block.Parent is QuoteBlock))
            {
                return TryParseSetexHeading(state, block);
            }
            return BlockState.Continue;
        }

        public override bool Close(BlockParserState state, Block block)
        {
            var paragraph = block as ParagraphBlock;
            var heading = block as HeadingBlock;
            if (paragraph != null)
            {
                var lines = paragraph.Lines;

                TryMatchLinkReferenceDefinition(lines, state);

                // If Paragraph is empty, we can discard it
                if (lines.Count == 0)
                {
                    return false;
                }

                var lineCount = lines.Count;
                for (int i = 0; i < lineCount; i++)
                {
                    lines.Lines[i].Slice.TrimStart();
                }
                if (lineCount > 0)
                {
                    lines.Lines[lineCount - 1].Slice.TrimEnd();
                }
            }
            else // if (heading?.Lines.Count > 1)
            {
                //heading.Lines.RemoveAt(heading.Lines.Count - 1);
            }

            return true;
        }

        private BlockState TryParseSetexHeading(BlockParserState state, Block block)
        {
            var paragraph = (ParagraphBlock) block;
            var headingChar = (char)0;
            bool checkForSpaces = false;
            var line = state.Line;
            var c = line.CurrentChar;
            while (c != '\0')
            {
                if (headingChar == 0)
                {
                    if (c == '=' || c == '-')
                    {
                        headingChar = c;
                        continue;
                    }
                    break;
                }

                if (checkForSpaces)
                {
                    if (!c.IsSpaceOrTab())
                    {
                        headingChar = (char)0;
                        break;
                    }
                }
                else if (c != headingChar)
                {
                    if (c.IsSpaceOrTab())
                    {
                        checkForSpaces = true;
                    }
                    else
                    {
                        headingChar = (char)0;
                        break;
                    }
                }
                c = line.NextChar();
            }

            if (headingChar != 0)
            {
                // We dicard the paragraph that will be transformed to a heading
                state.Discard(paragraph);

                // If we matched a LinkReferenceDefinition before matching the heading, and the remaining 
                // lines are empty, we can early exit and remove the paragraph
                if (!(TryMatchLinkReferenceDefinition(paragraph.Lines, state) && paragraph.Lines.Count == 0))
                {
                    var level = headingChar == '=' ? 1 : 2;

                    var heading = new HeadingBlock(this)
                    {
                        Column = paragraph.Column,
                        Level = level,
                        Lines = paragraph.Lines,
                    };
                    heading.Lines.Trim();

                    // Remove the paragraph as a pending block
                    state.NewBlocks.Push(heading);
                }
                return BlockState.BreakDiscard;
            }

            return BlockState.Continue;
        }

        private bool TryMatchLinkReferenceDefinition(StringLineGroup lines, BlockParserState state)
        {
            bool atLeastOneFound = false;

            var references = state.Document.GetLinkReferenceDefinitions();
            while (true)
            {
                // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
                var iterator = lines.ToCharIterator();
                LinkReferenceDefinition linkReferenceDefinition;
                if (LinkReferenceDefinition.TryParse(ref iterator, out linkReferenceDefinition))
                {
                    if (!references.ContainsKey(linkReferenceDefinition.Label))
                    {
                        references[linkReferenceDefinition.Label] = linkReferenceDefinition;
                    }
                    atLeastOneFound = true;

                    // Remove lines that have been matched
                    if (iterator.Start > iterator.End)
                    {
                        lines.Clear();
                    }
                    else
                    {
                        for (int i = iterator.SliceIndex - 1; i >= 0; i--)
                        {
                            lines.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return atLeastOneFound;
        }
    }
}