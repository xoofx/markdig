


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

                var localLineGroup = TempLineGroup;
                localLineGroup.Clear();

                if (paragraph == null)
                {
                    if (isBlankLine)
                    {
                        return MatchLineResult.None;
                    }
                }
                else
                {
                    foreach (var line in paragraph.Lines)
                    {
                        localLineGroup.Add(line);
                    }
                }

                if (!isBlankLine)
                {
                    localLineGroup.Add(liner);
                }

                // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
                LinkReferenceDefinitionBlock linkReferenceDefinition;
                if (LinkReferenceDefinitionBlock.TryParse(localLineGroup, out linkReferenceDefinition))
                {
                    // Remove the pending paragraph
                    if (paragraph != null)
                    {
                        var parent = (ContainerBlock)paragraph.Parent;
                        parent.Children.RemoveAt(parent.Children.Count - 1);
                    }

                    if (!state.Root.LinkReferenceDefinitions.ContainsKey(linkReferenceDefinition.Label))
                    {
                        state.Root.LinkReferenceDefinitions[linkReferenceDefinition.Label] = linkReferenceDefinition;
                    }

                    state.Block = null;

                    localLineGroup.Clear();
                    return MatchLineResult.LastDiscard;
                }
                localLineGroup.Clear();

                // We continue trying to match by default
                var result = MatchLineResult.Continue;
                if (paragraph == null)
                {
                    state.Block =  new ParagraphBlock();
                }
                else
                {
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
                            var level = headingChar == '=' ? 1 : 2;

                            var heading = new HeadingBlock
                            {
                                Level = level,
                                Inline = paragraph.Inline,
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

            public override void Close(Block block)
            {
                var paragraph = block as ParagraphBlock;
                if (paragraph != null)
                {
                    var lines = paragraph.Lines;
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