


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

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref MatchLineState state)
            {
                var liner = state.Line;
                liner.SkipLeadingSpaces3();

                // Else it is a continue, we don't break on blank lines
                var isBlankLine = liner.IsBlankLine();

                if (state.Block == null)
                {
                    if (isBlankLine)
                    {
                        return MatchLineResult.None;
                    }
                }
                else if (isBlankLine)
                {
                    return MatchLineResult.None;
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
                            if (!Utility.IsSpaceOrTab(c))
                            {
                                headingChar = (char)0;
                                break;
                            }
                        }
                        else if (c != headingChar)
                        {
                            if (Utility.IsSpaceOrTab(c))
                            {
                                checkForSpaces = true;
                            }
                        }
                    }

                    if (headingChar != 0)
                    {
                        var level = headingChar == '=' ? 1 : 2;

                        var paragraph = (ParagraphBlock) state.Block;
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
                    else
                    {
                        // Remove leading spaces from paragraph
                        var c = liner.Current;
                        while (Utility.IsSpaceOrTab(c))
                        {
                            c = liner.NextChar();
                        }
                    }
                }

                if (state.Block == null)
                {
                    state.Block = new ParagraphBlock();
                }

                return MatchLineResult.Continue;
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
                        var line = lines[i];
                        line.TrimStart();
                    }
                    if (lineCount > 0)
                    {
                        lines[lineCount - 1].TrimEnd();
                    }
                }
            }
        }
    }
}