


using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a paragraph.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class Paragraph : BlockLeaf
    {
        public static readonly BlockParser Parser = new ParserInternal();

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref MatchLineState state)
            {
                var liner = state.Liner;
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

                        state.Block = new Heading() {Level = level};
                        return MatchLineResult.LastDiscard;
                    }
                }

                if (state.Block == null)
                {
                    state.Block = new Paragraph();
                }

                return MatchLineResult.Continue;
            }
        }
    }
}