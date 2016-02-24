


using System;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a paragraph.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class ParagraphBlock : LeafBlock
    {
        public new static readonly BlockParser Parser = new ParserInternal();

        public ParagraphBlock(BlockParser parser) : base(parser)
        {
        }

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(BlockParserState state)
            {
                var liner = state.Line;
                liner.SkipLeadingSpaces3();

                var column = liner.Start;

                // Else it is a continue, we don't break on blank lines
                var isBlankLine = liner.IsBlankLine();

                var paragraph = state.Pending as ParagraphBlock;

                if (paragraph == null)
                {
                    if (isBlankLine)
                    {
                        return MatchLineResult.None;
                    }
                }

                // We continue trying to match by default
                var result = MatchLineResult.Continue;
                if (paragraph == null)
                {
                    state.NewBlocks.Push(new ParagraphBlock(this) { Column = column });
                }
                else
                {
                    if (isBlankLine)
                    {
                        result = MatchLineResult.None;
                    }
                    else if (!(paragraph.Parent is QuoteBlock))
                    {
                        var headingChar = (char) 0;
                        bool checkForSpaces = false;
                        for (int i = liner.Start; i <= liner.End; i++)
                        {
                            var c = liner[i];
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
                                    headingChar = (char) 0;
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
                            // If we matched a LinkReferenceDefinition before matching the heading, and the remaining 
                            // lines are empty, we can early exit and remove the paragraph
                            if (TryMatchLinkReferenceDefinition(paragraph.Lines, state) && paragraph.Lines.Count == 0)
                            {
                                state.Discard(paragraph);
                                return MatchLineResult.LastDiscard;
                            }

                            var level = headingChar == '=' ? 1 : 2;

                            var heading = new HeadingBlock(this)
                            {
                                Column = paragraph.Column,
                                Level = level,
                                Lines = paragraph.Lines,
                            };
                            foreach (var line in heading.Lines)
                            {
                                line.Trim();
                            }

                            // Remove the paragraph as a pending block
                            state.NewBlocks.Push(heading);
                            state.Discard(paragraph);

                            return MatchLineResult.LastDiscard;
                        }
                    }
                }

                return result;
            }

            private bool TryMatchLinkReferenceDefinition(StringLineGroup localLineGroup, BlockParserState state)
            {
                bool atLeastOneFound = false;

                var saved = new StringLineGroup.State();
                while (true)
                {
                    // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
                    localLineGroup.Save(ref saved);
                    LinkReferenceDefinitionBlock linkReferenceDefinition;
                    if (LinkReferenceDefinitionBlock.TryParse(localLineGroup, out linkReferenceDefinition))
                    {
                        if (!state.Root.LinkReferenceDefinitions.ContainsKey(linkReferenceDefinition.Label))
                        {
                            state.Root.LinkReferenceDefinitions[linkReferenceDefinition.Label] = linkReferenceDefinition;
                        }
                        atLeastOneFound = true;

                        // Remove lines that have been matched
                        if (localLineGroup.LinePosition == localLineGroup.Count)
                        {
                            localLineGroup.Clear();
                        }
                        else
                        {
                            for (int i = localLineGroup.LinePosition - 1; i >= 0; i--)
                            {
                                localLineGroup.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        if (!atLeastOneFound)
                        {
                            localLineGroup.Restore(ref saved);
                        }
                        break;
                    }
                }

                return atLeastOneFound;
            }

            public override void Close(BlockParserState state)
            {
                var paragraph = state.Pending as ParagraphBlock;
                var heading = state.Pending as HeadingBlock;
                if (paragraph != null)
                {
                    var lines = paragraph.Lines;

                    TryMatchLinkReferenceDefinition(lines, state);

                    // If Paragraph is empty, we can discard it
                    if (lines.Count == 0)
                    {
                        state.Pending = null;
                        return;
                    }

                    var lineCount = lines.Count;
                    for (int i = 0; i < lineCount; i++)
                    {
                        var line = lines[i];
                        line.Trim();
                    }
                }
                else if (heading?.Lines.Count > 1)
                {
                    //heading.Lines.RemoveAt(heading.Lines.Count - 1);
                }
            }
        }
    }
}