using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    public class BreakBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref MatchLineState state)
            {
                var liner = state.Liner;
                liner.SkipLeadingSpaces3();

                // 4.1 Thematic breaks 
                // A line consisting of 0-3 spaces of indentation, followed by a sequence of three or more matching -, _, or * characters, each followed optionally by any number of spaces
                var c = liner.Current;

                int count = 0;
                var matchChar = (char)0;
                while (!liner.IsEol)
                {
                    if (count == 0 && (c == '-' || c == '_' || c == '*'))
                    {
                        matchChar = c;
                        count++;
                    }
                    else if (c == matchChar)
                    {
                        count++;
                    }
                    else if (!Utility.IsSpace(c))
                    {
                        return MatchLineResult.None;
                    }
                    c = liner.NextChar();
                }

                if (count < 3)
                {
                    return MatchLineResult.None;
                }

                state.Block = new BreakBlock();
                return MatchLineResult.Last;
            }
        }
    }
}