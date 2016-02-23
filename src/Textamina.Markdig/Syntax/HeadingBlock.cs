using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    public class HeadingBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        public int Level { get; set; }

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(MatchLineState state)
            {
                var liner = state.Line;
                liner.SkipLeadingSpaces3();

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
                var c = liner.Current;

                int leadingCount = 0;
                for (; !liner.IsEol && leadingCount <= 6; leadingCount++)
                {
                    if (c != '#')
                    {
                        break;
                    }

                    c = liner.NextChar();
                }

                // closing # will be handled later, because anyway we have matched 

                // A space is required after leading #
                if (leadingCount > 0 && leadingCount <=6 && (CharHelper.IsSpace(c) || liner.IsEol))
                {
                    liner.NextChar();
                    state.NewBlocks.Push(new HeadingBlock() {Level = leadingCount});


                    // The optional closing sequence of #s must be preceded by a space and may be followed by spaces only.
                    int endState = 0;
                    int countClosingTags = 0;
                    for (int i = liner.End; i >= liner.Start - 1; i--)  // Go up to Start - 1 in order to match the space after the first ###
                    {
                        c = liner[i];
                        if (endState == 0)
                        {
                            if (CharHelper.IsSpace(c)) // TODO: Not clear if it is a space or space+tab in the specs
                            {
                                continue;
                            }
                            endState = 1;
                        }
                        if (endState == 1)
                        {
                            if (c == '#')
                            {
                                countClosingTags++;
                                continue;
                            }

                            if (countClosingTags > 0)
                            {
                                if (CharHelper.IsSpace(c))
                                {
                                    liner.End = i - 1;
                                }
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    return MatchLineResult.Last;
                }

                return MatchLineResult.None;
            }

            public override void Close(MatchLineState state)
            {
                var heading = (HeadingBlock) state.Pending;
                heading.Lines.Trim();
            }
        }
    }
}