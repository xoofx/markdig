using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class QuoteBlock : ContainerBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(MatchLineState state)
            {
                var liner = state.Line;

                liner.SkipLeadingSpaces3();

                // 5.1 Block quotes 
                // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
                var c = liner.Current;
                if (c != '>')
                {
                    if (state.Block != null && liner.IsBlankLine())
                    {
                        return MatchLineResult.LastDiscard;
                    }
                    return MatchLineResult.None;
                }

                c = liner.NextChar();
                if (c.IsSpace())
                {
                    liner.NextChar();
                }

                if (state.Block == null)
                {
                    state.Block = new QuoteBlock();
                }

                return MatchLineResult.Continue;
            }
        }
    }
}