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
            return state.IsBlankLine ? BlockState.BreakDiscard : BlockState.Continue;
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
                    lines.Slices[i].TrimStart();
                }
                if (lineCount > 0)
                {
                    lines.Slices[lineCount - 1].TrimEnd();
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
            for (int i = state.Start; i <= state.EndOffset; i++)
            {
                var c = state.Line[i];
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
            }

            if (headingChar != 0)
            {
                state.Discard(paragraph);

                // If we matched a LinkReferenceDefinition before matching the heading, and the remaining 
                // lines are empty, we can early exit and remove the paragraph
                if (!TryMatchLinkReferenceDefinition(paragraph.Lines, state) && paragraph.Lines.Count == 0)
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

        private bool TryMatchLinkReferenceDefinition(StringSliceList localLineGroup, BlockParserState state)
        {
            bool atLeastOneFound = false;

            //var saved = new StringSliceList.State();
            //while (true)
            //{
            //    // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
            //    localLineGroup.Save(ref saved);
            //    LinkReferenceDefinitionBlock linkReferenceDefinition;
            //    if (LinkReferenceDefinitionBlock.TryParse(localLineGroup, out linkReferenceDefinition))
            //    {
            //        if (!state.Root.LinkReferenceDefinitions.ContainsKey(linkReferenceDefinition.Label))
            //        {
            //            state.Root.LinkReferenceDefinitions[linkReferenceDefinition.Label] = linkReferenceDefinition;
            //        }
            //        atLeastOneFound = true;

            //        // Remove lines that have been matched
            //        if (localLineGroup.LinePosition == localLineGroup.Count)
            //        {
            //            localLineGroup.Clear();
            //        }
            //        else
            //        {
            //            for (int i = localLineGroup.LinePosition - 1; i >= 0; i--)
            //            {
            //                localLineGroup.RemoveAt(i);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (!atLeastOneFound)
            //        {
            //            localLineGroup.Restore(ref saved);
            //        }
            //        break;
            //    }
            //}

            return atLeastOneFound;
        }
    }
}