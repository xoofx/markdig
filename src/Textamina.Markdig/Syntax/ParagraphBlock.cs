


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
        public static readonly BlockParser Parser = new ParserInternal();

        [ThreadStatic]
        private static StringLineGroup TempLineGroup = new StringLineGroup();

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(MatchLineState state)
            {
                var liner = state.Line;
                liner.SkipLeadingSpaces3();

                // Else it is a continue, we don't break on blank lines
                var isBlankLine = liner.IsBlankLine();

                var paragraph = state.Block as ParagraphBlock;

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
                    state.Block =  new ParagraphBlock();
                }
                else
                {
                    var localLineGroup = TempLineGroup;
                    localLineGroup.Clear();

                    foreach (var line in paragraph.Lines)
                    {
                        localLineGroup.Add(line);
                    }

                    if (!isBlankLine)
                    {
                        localLineGroup.Add(liner);
                    }

                    if (isBlankLine)
                    {
                        result = MatchLineResult.None;
                    }
                    else
                    {
                        var headingChar = (char) 0;
                        bool checkForSpaces = false;
                        for (int i = liner.Start; i < liner.End; i++)
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
                            }
                        }

                        if (headingChar != 0)
                        {
                            // If we matched a LinkReferenceDefinition before matching the heading, and the remaining 
                            // lines are empty, we can early exit and remove the paragraph
                            if (TryMatchLinkReferenceDefinition(paragraph.Lines, state) && paragraph.Lines.Count == 0)
                            {
                                RemoveParagraph(paragraph, state);
                                return MatchLineResult.LastDiscard;
                            }

                            var level = headingChar == '=' ? 1 : 2;

                            var heading = new HeadingBlock
                            {
                                Level = level,
                                Lines = paragraph.Lines,
                                Parent = paragraph.Parent
                            };
                            state.Block = heading;

                            // Replace the children in the parent
                            var parent = (ContainerBlock) paragraph.Parent;
                            parent.Children[parent.Children.Count - 1] = heading;

                            return MatchLineResult.LastDiscard;
                        }
                    }
                }

                return result;
            }

            private void RemoveParagraph(ParagraphBlock paragraph, MatchLineState state)
            {
                // Remove the pending paragraph
                if (paragraph != null)
                {
                    var parent = (ContainerBlock)paragraph.Parent;
                    parent.Children.RemoveAt(parent.Children.Count - 1);
                }
                state.Block = null;
            }

            private bool TryMatchLinkReferenceDefinition(StringLineGroup localLineGroup, MatchLineState state)
            {
                bool atLeastOneFound = false;

                while (true)
                {
                    // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
                    var saved = localLineGroup.Save();
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

            public override void Close(MatchLineState state)
            {
                var paragraph = state.Block as ParagraphBlock;
                if (paragraph != null)
                {
                    var lines = paragraph.Lines;

                    TryMatchLinkReferenceDefinition(lines, state);

                    // If Paragraph is empty, we can discard it
                    if (lines.Count == 0)
                    {
                        state.Block = null;
                        return;
                    }

                    var lineCount = lines.Count;
                    for (int i = 0; i < lineCount; i++)
                    {
                        lines[i].Trim();
                    }
                }
            }
        }
    }
}